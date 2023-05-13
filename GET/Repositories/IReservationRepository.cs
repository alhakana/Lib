using GET.Entities;

namespace GET.Repositories
{
    public interface IReservationRepository
    {
        Task<IEnumerable<Reservation>> GetAllReservationsAsync();
        Task<Reservation> GetReservationByIdAsync(int id);
        Task<IEnumerable<Reservation>> GetReservationsByUserIdAsync(string userId);
        Task<Reservation> AddReservationAsync(Reservation reservation);
        Task<Reservation> UpdateReservationAsync(Reservation reservation);
        Task DeleteReservationAsync(int id);
    }
}

