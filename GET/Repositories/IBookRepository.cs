using System;
using GET.Entities;

namespace GET.Repositories
{
	public interface IBookRepository
	{
        Task<IEnumerable<Book>> GetAllBooksAsync();
        Task<Book> GetBookByIdAsync(int id);
        Task<Book> AddBookAsync(Book book);
        Task<Book> UpdateBookAsync(Book book);
        Task DeleteBookAsync(int id);
        Task<int> GetAvailableCopiesAsync(int bookId);
    }
}

