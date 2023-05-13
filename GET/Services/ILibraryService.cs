using GET.Entities;

namespace GET.Services
{
    public interface ILibraryService
    {
        // Book-related methods
        Task<IEnumerable<Book>> GetAllBooksAsync();
        Task<Book> GetBookByIdAsync(int id);
        Task<Book> AddBookAsync(Book book);
        Task<Book> UpdateBookAsync(Book book);
        Task DeleteBookAsync(int id);

        // Reservation-related methods
        Task<IEnumerable<Reservation>> GetAllReservationsAsync();
        Task<Reservation> GetReservationByIdAsync(int id);
        Task<IEnumerable<Reservation>> GetReservationsByUserIdAsync(string userId);
        Task<Reservation> AddReservationAsync(Reservation reservation);
        Task<Reservation> UpdateReservationAsync(Reservation reservation);
        Task DeleteReservationAsync(int id);
        Task<Reservation> RequestReservationAsync(int bookId, string userId);
        Task<bool> ApproveReservationAsync(int reservationId);
        Task<bool> RejectReservationAsync(int reservationId);
        Task<bool> CancelReservationAsync(int reservationId, string userId);
        Task<bool> ReturnBookAsync(int reservationId);

    }
}

