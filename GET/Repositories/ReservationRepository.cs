using System;
using GET.Data;
using GET.Entities;
using GET.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GET.RepositoriesImplementation
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly LibraryDbContext _context;

        public ReservationRepository(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Reservation>> GetAllReservationsAsync()
        {
            return await _context.Reservations
                                 .Include(r => r.Book)
                                 .Include(r => r.User)
                                 .ToListAsync();
        }

        public async Task<Reservation> GetReservationByIdAsync(int id)
        {
            return await _context.Reservations
                                 .Include(r => r.Book)
                                 .Include(r => r.User)
                                 .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Reservation>> GetReservationsByUserIdAsync(string userId)
        {
            return await _context.Reservations
                                 .Include(r => r.Book)
                                 .Include(r => r.User)
                                 .Where(r => r.UserId == userId)
                                 .ToListAsync();
        }

        public async Task<Reservation> AddReservationAsync(Reservation reservation)
        {
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();
            return reservation;
        }

        public async Task<Reservation> UpdateReservationAsync(Reservation reservation)
        {
            _context.Entry(reservation).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return reservation;
        }

        public async Task DeleteReservationAsync(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
                await _context.SaveChangesAsync();
            }
        }

    }
}

