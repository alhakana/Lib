namespace GET.Entities
{
    public class Book
	{
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }
        public ICollection<Reservation> Reservations { get; set; }
    }
}