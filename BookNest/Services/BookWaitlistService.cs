using System.Data;
using BookNest.DatabaseContext;
using BookNest.DTO;
using BookNest.IServices;
using BookNest.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BookNest.Services
{
    public class BookWaitlistService : IBookWaitlistService
    {
        private readonly AppDbContext _dbContext;
        private readonly WaitlistOptions _options;

        public BookWaitlistService(AppDbContext dbContext, IOptions<WaitlistOptions> options)
        {
            _dbContext = dbContext;
            _options = options.Value;
        }

        public async Task<string> JoinQueueAsync(string username, int bookId)
        {
            await ProcessExpiredOffersAsync();

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            var book = await _dbContext.Books.SingleOrDefaultAsync(b => b.BookId == bookId);
            if (book == null)
                return "BookNotFound";

            var existing = await _dbContext.BookWaitlistEntries.AnyAsync(entry =>
                entry.BookId == bookId && entry.Username == username &&
                (entry.Status == "Waiting" || entry.Status == "Offered" || entry.Status == "Accepted"));
            if (existing)
                return "AlreadyQueued";

            var entry = new BookWaitlistEntry
            {
                BookId = bookId,
                Username = username,
                Status = "Waiting",
                JoinedAt = DateTime.UtcNow
            };
            await _dbContext.BookWaitlistEntries.AddAsync(entry);
            await _dbContext.SaveChangesAsync();
            await OfferAvailableCopiesCoreAsync(book);
            await transaction.CommitAsync();
            return entry.Status;
        }

        public async Task<List<BookWaitlistEntryDto>> GetForUserAsync(string username)
        {
            await ProcessExpiredOffersAsync();

            var entries = await (from entry in _dbContext.BookWaitlistEntries.AsNoTracking()
                                 join book in _dbContext.Books.AsNoTracking() on entry.BookId equals book.BookId
                                 where entry.Username == username
                                 orderby entry.JoinedAt descending
                                 select new BookWaitlistEntryDto
                                 {
                                     WaitlistId = entry.WaitlistId,
                                     BookId = entry.BookId,
                                     Username = entry.Username,
                                     Title = book.Title,
                                     Status = entry.Status,
                                     JoinedAt = entry.JoinedAt,
                                     ExpiresAt = entry.ExpiresAt
                                 }).ToListAsync();

            foreach (var entry in entries.Where(item => item.Status == "Waiting"))
            {
                entry.Position = await _dbContext.BookWaitlistEntries.CountAsync(item =>
                    item.BookId == entry.BookId && item.Status == "Waiting" &&
                    (item.JoinedAt < entry.JoinedAt ||
                     (item.JoinedAt == entry.JoinedAt && item.WaitlistId <= entry.WaitlistId)));
            }

            return entries;
        }

        public async Task<int> GetNotificationCountAsync(string username)
        {
            await ProcessExpiredOffersAsync();
            return await _dbContext.BookWaitlistEntries.CountAsync(entry =>
                entry.Username == username && entry.Status == "Offered");
        }

        public async Task<List<BookWaitlistEntryDto>> GetActiveOffersAsync()
        {
            await ProcessExpiredOffersAsync();

            return await (from entry in _dbContext.BookWaitlistEntries.AsNoTracking()
                          join book in _dbContext.Books.AsNoTracking() on entry.BookId equals book.BookId
                          where entry.Status == "Offered" || entry.Status == "Accepted"
                          orderby entry.ExpiresAt
                          select new BookWaitlistEntryDto
                          {
                              WaitlistId = entry.WaitlistId,
                              BookId = entry.BookId,
                              Username = entry.Username,
                              Title = book.Title,
                              Status = entry.Status,
                              JoinedAt = entry.JoinedAt,
                              ExpiresAt = entry.ExpiresAt
                          }).ToListAsync();
        }

        public async Task<bool> AcceptOfferAsync(string username, int waitlistId)
        {
            await ProcessExpiredOffersAsync();

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            var entry = await _dbContext.BookWaitlistEntries.SingleOrDefaultAsync(item =>
                item.WaitlistId == waitlistId && item.Username == username);
            if (entry == null || entry.Status != "Offered" || entry.ExpiresAt <= DateTime.UtcNow)
                return false;

            entry.Status = "Accepted";
            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }

        public async Task<bool> CancelEntryAsync(string username, int waitlistId)
        {
            await ProcessExpiredOffersAsync();

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            var entry = await _dbContext.BookWaitlistEntries.SingleOrDefaultAsync(item =>
                item.WaitlistId == waitlistId && item.Username == username);
            if (entry == null || (entry.Status != "Waiting" && entry.Status != "Offered" && entry.Status != "Accepted"))
                return false;

            var releasesReservedCopy = entry.Status == "Offered" || entry.Status == "Accepted";
            entry.Status = "Cancelled";

            if (releasesReservedCopy)
            {
                var book = await _dbContext.Books.SingleOrDefaultAsync(item => item.BookId == entry.BookId);
                if (book != null)
                {
                    book.AvailableQuantity++;
                    await OfferAvailableCopiesCoreAsync(book);
                }
            }
            else
            {
                await _dbContext.SaveChangesAsync();
            }

            await transaction.CommitAsync();
            return true;
        }

        public async Task<bool> MarkCollectedAsync(int waitlistId)
        {
            await ProcessExpiredOffersAsync();

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            var entry = await _dbContext.BookWaitlistEntries.SingleOrDefaultAsync(item => item.WaitlistId == waitlistId);
            if (entry == null || entry.Status != "Accepted" || entry.ExpiresAt <= DateTime.UtcNow)
                return false;

            var book = await _dbContext.Books.SingleOrDefaultAsync(item => item.BookId == entry.BookId);
            if (book == null)
                return false;

            var now = DateTime.UtcNow;
            var order = new Order
            {
                OrderCode = "ORD-" + now.ToString("yyyyMMdd") + "-" + Guid.NewGuid().ToString("N")[..6],
                Username = entry.Username,
                OrderDate = now,
                Status = "Approved",
                ReturnDate = now.AddDays(7),
                ReturnStatus = "Pending",
                Action = true
            };
            await _dbContext.Orders.AddAsync(order);
            await _dbContext.SaveChangesAsync();

            await _dbContext.OrderItems.AddAsync(new OrderItem
            {
                OrderId = order.OrderId,
                BookId = book.BookId,
                Title = book.Title,
                ImageUrl = book.ImageUrl,
                Quantity = 1,
                Status = "Approved",
                ReturnStatus = "Pending",
                Action = true
            });

            entry.Status = "Collected";
            entry.CollectedAt = now;
            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }

        public async Task OfferAvailableCopiesAsync(int bookId)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            var book = await _dbContext.Books.SingleOrDefaultAsync(item => item.BookId == bookId);
            if (book == null)
                return;

            await OfferAvailableCopiesCoreAsync(book);
            await transaction.CommitAsync();
        }

        public async Task ProcessExpiredOffersAsync()
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            var now = DateTime.UtcNow;
            var expiredEntries = await _dbContext.BookWaitlistEntries
                .Where(entry => (entry.Status == "Offered" || entry.Status == "Accepted") && entry.ExpiresAt <= now)
                .ToListAsync();

            var returnedBookIds = expiredEntries.Select(entry => entry.BookId).Distinct().ToList();
            var returnedBooks = await _dbContext.Books
                .Where(book => returnedBookIds.Contains(book.BookId))
                .ToDictionaryAsync(book => book.BookId);

            foreach (var entry in expiredEntries)
            {
                entry.Status = "Expired";
                if (returnedBooks.TryGetValue(entry.BookId, out var book))
                    book.AvailableQuantity++;
            }

            var booksWithWaitingEntries = await _dbContext.Books
                .Where(book => book.AvailableQuantity > 0 &&
                    _dbContext.BookWaitlistEntries.Any(entry =>
                        entry.BookId == book.BookId && entry.Status == "Waiting"))
                .ToListAsync();

            foreach (var book in booksWithWaitingEntries)
                await OfferAvailableCopiesCoreAsync(book);

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }

        private async Task OfferAvailableCopiesCoreAsync(Book book)
        {
            if (book.AvailableQuantity <= 0)
                return;

            var waitingEntries = await _dbContext.BookWaitlistEntries
                .Where(entry => entry.BookId == book.BookId && entry.Status == "Waiting")
                .OrderBy(entry => entry.JoinedAt)
                .ThenBy(entry => entry.WaitlistId)
                .ToListAsync();

            foreach (var entry in waitingEntries)
            {
                if (book.AvailableQuantity <= 0)
                    break;

                entry.Status = "Offered";
                entry.OfferedAt = DateTime.UtcNow;
                entry.ExpiresAt = entry.OfferedAt.Value.AddHours(_options.OfferDurationHours);
                book.AvailableQuantity--;
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
