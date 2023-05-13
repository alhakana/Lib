namespace GET.Entities
{
    public class Reservation
    {
        public int Id { get; set; }
        public DateTime ReservationDate { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsReturned { get; set; }
        public ReservationStatus Status { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}

