using GET.Hubs;
using GET.Services;
using GET.ViewModels.Mapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace GET.Controllers
{
    [Authorize]
    public class ReservationController : Controller
    {
        private readonly ILibraryService _libraryService;
        private readonly IHubContext<LibraryHub> _hubContext;

        public ReservationController(ILibraryService libraryService, IHubContext<LibraryHub> hubContext)
        {
            _libraryService = libraryService;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var reservations = await _libraryService.GetReservationsByUserIdAsync(userId);

            return View(reservations.Select(reservation => reservation.ToViewModel()));
        }

        [Authorize(Roles = "Librarian")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveReservation(int reservationId)
        {
            bool isApproved = await _libraryService.ApproveReservationAsync(reservationId);

            if (isApproved)
            {
                // Retrieve the reservation object to send it to the SignalR hub
                var reservation = await _libraryService.GetReservationByIdAsync(reservationId);
                var userId = reservation.UserId;
                await _hubContext.Clients.Users(userId).SendAsync("ApproveReservation", reservation.ToViewModel());
            }

            return RedirectToAction(nameof(AllReservations));
        }

        [HttpPost]
        [Authorize(Roles = "Librarian")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectReservation(int reservationId)
        {
            var reservation = await _libraryService.GetReservationByIdAsync(reservationId);
            var userId = reservation.UserId;

            bool isRejected = await _libraryService.RejectReservationAsync(reservationId);
            if (isRejected)
            {
                await _hubContext.Clients.Users(userId).SendAsync("RejectReservation", reservationId);
            }

            return RedirectToAction(nameof(AllReservations));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelReservation(int reservationId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _libraryService.CancelReservationAsync(reservationId, userId);

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> AllReservations()
        {
            var reservations = await _libraryService.GetAllReservationsAsync();

            return View(reservations.Select(reservation => reservation.ToViewModel()));
        }

        [Authorize(Roles = "Librarian")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReturnBook(int reservationId)
        {
            bool isReturned = await _libraryService.ReturnBookAsync(reservationId);
            return RedirectToAction(nameof(AllReservations));
        }
    }
}

