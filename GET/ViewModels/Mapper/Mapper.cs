using GET.Entities;

namespace GET.ViewModels.Mapper
{
    public static class Mapper
	{
		public static ReservationViewModel ToViewModel(this Reservation reservation)
		{
            return new ReservationViewModel()
            {
                Id = reservation.Id,
                BookTitle = reservation.Book?.Title ?? "N/A",
                BookAuthor = reservation.Book?.Author ?? "N/A",
                UserName = reservation.User?.UserName ?? "N/A",
                ReservationDate = reservation.ReservationDate,
                DueDate = reservation.DueDate,
                IsReturned = reservation.IsReturned,
                Status = reservation.Status
            };
		}

        public static BookViewModel ToViewModel(this Book book)
        {
            return new BookViewModel()
            {
                Id = book.Id,
                Author = book.Author,
                Title = book.Title,
                AvailableCopies = book.AvailableCopies,
                TotalCopies = book.TotalCopies
            };
        }

        public static Book ToModel(this BookViewModel book)
        {
            return new Book()
            {
                Id = book.Id,
                Author = book.Author,
                Title = book.Title,
                AvailableCopies = book.AvailableCopies,
                TotalCopies = book.TotalCopies
            };
        }
    }
}

