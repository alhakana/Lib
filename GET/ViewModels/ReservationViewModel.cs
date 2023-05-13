using GET.Entities;

namespace GET.ViewModels
{
    public class ReservationViewModel
    {
        public int Id { get; set; }
        public string BookTitle { get; set; }
        public string BookAuthor { get; set; }
        public string UserName { get; set; }
        public DateTime ReservationDate { get; set; }
        public DateTime DueDate { get; set; }
        public ReservationStatus Status { get; set; }
        public bool IsReturned { get; set; }
    }
}
