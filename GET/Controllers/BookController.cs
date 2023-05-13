using GET.Hubs;
using GET.Entities;
using GET.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using GET.ViewModels.Mapper;
using GET.ViewModels;

namespace GET.Controllers
{
    public class BookController : Controller
    {
        private readonly ILibraryService _libraryService;
        private readonly IHubContext<LibraryHub> _hubContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookController(UserManager<ApplicationUser> userManager, ILibraryService libraryService, IHubContext<LibraryHub> hubContext)
        {
            _userManager = userManager;
            _libraryService = libraryService;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Index()
        {
            var books = await _libraryService.GetAllBooksAsync();
            return View(books.Select(book => book.ToViewModel()));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookViewModel book)
        {
            if (ModelState.IsValid)
            {
                await _libraryService.AddBookAsync(book.ToModel());
                return RedirectToAction(nameof(Index));
            }

            return View(book);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var book = await _libraryService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book.ToViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BookViewModel book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _libraryService.UpdateBookAsync(book.ToModel());
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var book = await _libraryService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book.ToViewModel());
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _libraryService.DeleteBookAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reserve(int bookId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = _userManager.GetUserId(User);
            var reservations = await _libraryService.GetReservationsByUserIdAsync(userId);
            if (reservations.Count(reservation => reservation.Status == ReservationStatus.PENDING || reservation.Status == ReservationStatus.APPROVED) > 4)
            {
                // Error message
                TempData["ErrorMessage"] = "You have reached the maximum number of reservations.";
                return RedirectToAction("Index");
            }

            var reservation = await _libraryService.RequestReservationAsync(bookId, userId);

            if (reservation != null)
            {
                var librerianIds = (await _userManager.GetUsersInRoleAsync("Librarian"))
                        .Select(l => l.Id);

                await _hubContext.Clients.Users(librerianIds).SendAsync("CreateReservation", reservation.ToViewModel());

                // Success message
                TempData["SuccessMessage"] = "Reservation successful!";
            }
            
            return RedirectToAction("Index");
        }
    }
}