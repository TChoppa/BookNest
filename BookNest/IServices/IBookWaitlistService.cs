using BookNest.DTO;

namespace BookNest.IServices
{
    public interface IBookWaitlistService
    {
        Task<string> JoinQueueAsync(string username, int bookId);
        Task<List<BookWaitlistEntryDto>> GetForUserAsync(string username);
        Task<int> GetNotificationCountAsync(string username);
        Task<List<BookWaitlistEntryDto>> GetActiveOffersAsync();
        Task<bool> AcceptOfferAsync(string username, int waitlistId);
        Task<bool> CancelEntryAsync(string username, int waitlistId);
        Task<bool> MarkCollectedAsync(int waitlistId);
        Task OfferAvailableCopiesAsync(int bookId);
        Task ProcessExpiredOffersAsync();
    }
}
