using BookStore.Data;
using BookStore.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Repositories
{
    public class HomeRepository : IHomeRepository
    {
        private readonly ApplicationDbContext _db;

        public HomeRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Genre>> Genres()
        {
            return await _db.Genres.ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetBooks(string sTerm = "", int genreId = 0)
        {
            sTerm = sTerm.ToLower();
            IEnumerable<Book> books = await (from book in _db.Book
                                             join genre in _db.Genres
                                             on book.GenreId equals genre.Id
                                             where string.IsNullOrWhiteSpace(sTerm) || (book != null && book.Title.ToLower().StartsWith(sTerm))
                                             select new Book
                                             {
                                                 Id = book.Id,
                                                 Title = book.Title,
                                                 Author = book.Author,
                                                 Description = book.Description,
                                                 Price = book.Price,
                                                 TotalPage = book.TotalPage,
                                                 BookCoverUrl = book.BookCoverUrl,
                                                 GenreId = book.GenreId,
                                                 GenreName = genre.GenreName
                                             }
                         ).ToListAsync();
            if (genreId > 0)
            {

                books = books.Where(a => a.GenreId == genreId).ToList();
            }
            return books;
        }
    }
}
