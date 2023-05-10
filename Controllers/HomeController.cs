using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using BookStore.Repositories;
using BookStore.Models.DTO;
using System.IO;
using static System.Reflection.Metadata.BlobBuilder;

namespace BookStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomeRepository _homeRepository;
        private readonly ApplicationDbContext _context;


        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IHomeRepository homeRepository)
        {
            _logger = logger;
            _context = context;
            _homeRepository = homeRepository;
        }

        public async Task<IActionResult> Index(string sterm="", int genreId = 0)
        {
            IEnumerable<Book> books = await _homeRepository.GetBooks(sterm, genreId);
            IEnumerable<Genre> genres = await _homeRepository.Genres();
            BookDisplayModel bookModel = new BookDisplayModel
            {
                Books = books,
                Genres = genres,
                STerm = sterm,
                GenreId = genreId
            };
            return View(bookModel);
        }

        public async Task<IActionResult> Details(int? id, string sterm = "", int genreId = 0)
        {

            if (id == null || _context.Book == null)
            {
                return NotFound();
            }
            IEnumerable<Book> books = await _homeRepository.GetBooks(sterm, genreId);
            IEnumerable<Genre> genres = await _homeRepository.Genres();
            var book = await _context.Book.FirstOrDefaultAsync(m => m.Id == id);


            return View(book);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}