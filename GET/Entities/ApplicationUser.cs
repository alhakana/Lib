using Microsoft.AspNetCore.Identity;

namespace GET.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsLibrarian { get; set; }
        public ICollection<Reservation> Reservations { get; set; }
    }
}

