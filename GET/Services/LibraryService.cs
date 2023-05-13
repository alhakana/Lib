using GET.Entities;
using GET.Repositories;
using GET.Services;
using Microsoft.AspNetCore.Identity;

namespace GET.ServicesImplementation
{
    public class LibraryService : ILibraryService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IReservationRepository _reservationRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public LibraryService(UserManager<ApplicationUser> userManager,IBookRepository bookRepository, IReservationRepository reservationRepository)
        {
            _userManager = userManager;
            _bookRepository = bookRepository;
            _reservationRepository = reservationRepository;
        }

        // Book-related methods
        public async Task<IEnumerable<Book>> GetAllBooksAsync() => await _bookRepository.GetAllBooksAsync();
        public async Task<Book> GetBookByIdAsync(int id) => await _bookRepository.GetBookByIdAsync(id);
        public async Task<Book> UpdateBookAsync(Book book) => await _bookRepository.UpdateBookAsync(book);
        public async Task DeleteBookAsync(int id) => await _bookRepository.DeleteBookAsync(id);

        public async Task<Book> AddBookAsync(Book book)
        {
            book.AvailableCopies = book.TotalCopies;
            return await _bookRepository.AddBookAsync(book);
        }
        
        // Reservation-related methods
        public async Task<IEnumerable<Reservation>> GetAllReservationsAsync() => await _reservationRepository.GetAllReservationsAsync();
        public async Task<Reservation> GetReservationByIdAsync(int id) => await _reservationRepository.GetReservationByIdAsync(id);
        public async Task<IEnumerable<Reservation>> GetReservationsByUserIdAsync(string userId) => await _reservationRepository.GetReservationsByUserIdAsync(userId);
        public async Task<Reservation> AddReservationAsync(Reservation reservation) => await _reservationRepository.AddReservationAsync(reservation);
        public async Task<Reservation> UpdateReservationAsync(Reservation reservation) => await _reservationRepository.UpdateReservationAsync(reservation);
        public async Task DeleteReservationAsync(int id) => await _reservationRepository.DeleteReservationAsync(id);

        public async Task<Reservation> RequestReservationAsync(int bookId, string userId)
        {
            var availableCopies = await _bookRepository.GetAvailableCopiesAsync(bookId);

            if (availableCopies > 0)
            {
                var reservation = new Reservation
                {
                    BookId = bookId,
                    UserId = userId,
                    ReservationDate = DateTime.UtcNow,
                    // DueDate is not set here
                    IsReturned = false,
                    Status = ReservationStatus.PENDING
                };
                var user = await _userManager.FindByIdAsync(userId);
                reservation.User = user;

                await _reservationRepository.AddReservationAsync(reservation);

                return reservation;
            }

            return null;
        }

        public async Task<bool> ApproveReservationAsync(int reservationId)
        {
            var reservation = await _reservationRepository.GetReservationByIdAsync(reservationId);

            if (reservation != null)
            {
                reservation.DueDate = DateTime.UtcNow.AddDays(14); // Set the DueDate when approving the reservation
                reservation.Status = ReservationStatus.APPROVED;

                // Decrease the available copies of the book
                var book = await _bookRepository.GetBookByIdAsync(reservation.BookId);
                if (book != null && book.AvailableCopies > 0)
                {
                    book.AvailableCopies -= 1;
                }
                else
                {
                    return false;
                }

                await _bookRepository.UpdateBookAsync(book);
                await _reservationRepository.UpdateReservationAsync(reservation);

                return true;
            }

            return false;
        }

        public async Task<bool> RejectReservationAsync(int reservationId)
        {
            var reservation = await _reservationRepository.GetReservationByIdAsync(reservationId);

            if (reservation != null)
            {
                reservation.Status = ReservationStatus.REJECTED;

                await _reservationRepository.UpdateReservationAsync(reservation);
                return true;
            }

            return false;
        }


        public async Task<bool> CancelReservationAsync(int reservationId, string userId)
        {
            var reservation = await _reservationRepository.GetReservationByIdAsync(reservationId);

            if (reservation == null || reservation.UserId != userId)
            {
                return false;
            }

            await _reservationRepository.DeleteReservationAsync(reservationId);

            return true;
        }

        public async Task<bool> ReturnBookAsync(int reservationId)
        {
            var reservation = await _reservationRepository.GetReservationByIdAsync(reservationId);

            if (reservation != null && !reservation.IsReturned)
            {
                reservation.IsReturned = true;

                var book = await _bookRepository.GetBookByIdAsync(reservation.BookId);
                if (book != null)
                {
                    book.AvailableCopies += 1;
                }

                await _bookRepository.UpdateBookAsync(book);
                await _reservationRepository.UpdateReservationAsync(reservation);

                return true;
            }

            return false;
        }
    }

}

