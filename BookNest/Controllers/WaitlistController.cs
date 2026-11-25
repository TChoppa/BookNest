using BookNest.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookNest.Controllers
{
    [Authorize]
    public class WaitlistController : Controller
    {
        private readonly IBookWaitlistService _waitlistService;

        public WaitlistController(IBookWaitlistService waitlistService)
        {
            _waitlistService = waitlistService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Librarian"))
            {
                ViewBag.IsLibrarian = true;
                ViewBag.PageTitle = "Waitlist Offers";
                return View(await _waitlistService.GetActiveOffersAsync());
            }

            if (!User.IsInRole("Student"))
                return Forbid();

            ViewBag.IsLibrarian = false;
            ViewBag.PageTitle = "My Book Waitlist";
            return View(await _waitlistService.GetForUserAsync(User.Identity!.Name!));
        }

        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Join(int bookId)
        {
            if (bookId <= 0)
                return BadRequest(new { success = false, message = "Invalid book." });

            var result = await _waitlistService.JoinQueueAsync(User.Identity!.Name!, bookId);
            return result switch
            {
                "BookNotFound" => NotFound(new { success = false, message = "Book not found." }),
                "AlreadyQueued" => Conflict(new { success = false, message = "You already have an active place in this book's queue." }),
                "Offered" => Ok(new { success = true, message = "A copy is available and reserved for you. Check My Waitlist to accept it." }),
                _ => Ok(new { success = true, message = "You joined the book waitlist." })
            };
        }

        [HttpGet]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetNotificationCount()
        {
            var count = await _waitlistService.GetNotificationCountAsync(User.Identity!.Name!);
            return Ok(count);
        }

        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Accept(int waitlistId)
        {
            if (waitlistId <= 0)
                return BadRequest(new { success = false, message = "Invalid waitlist entry." });

            var accepted = await _waitlistService.AcceptOfferAsync(User.Identity!.Name!, waitlistId);
            if (!accepted)
                return Conflict(new { success = false, message = "This offer is no longer available." });

            return Ok(new { success = true, message = "Offer accepted. Please collect the book before the expiry time." });
        }

        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Cancel(int waitlistId)
        {
            if (waitlistId <= 0)
                return BadRequest(new { success = false, message = "Invalid waitlist entry." });

            var cancelled = await _waitlistService.CancelEntryAsync(User.Identity!.Name!, waitlistId);
            if (!cancelled)
                return Conflict(new { success = false, message = "This waitlist entry can no longer be cancelled." });

            return Ok(new { success = true, message = "Waitlist entry cancelled." });
        }

        [HttpPost]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> MarkCollected(int waitlistId)
        {
            if (waitlistId <= 0)
                return BadRequest(new { success = false, message = "Invalid waitlist entry." });

            var collected = await _waitlistService.MarkCollectedAsync(waitlistId);
            if (!collected)
                return Conflict(new { success = false, message = "The student must accept an active offer before collection can be recorded." });

            return Ok(new { success = true });
        }
    }
}
