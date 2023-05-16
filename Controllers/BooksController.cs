using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookStore.Data;
using BookStore.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authorization;

namespace BookStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment webHost;

        public BooksController(ApplicationDbContext context, IWebHostEnvironment webHost)
        {
            _context = context;
            this.webHost = webHost;
        }

        public async Task<IActionResult> SelectedGenre()
        {
            var instructors = await _context.Genres.ToListAsync();
            return PartialView("~/Views/Genres/Selected.cshtml", instructors);
        }

        public async Task<IActionResult> SelectedEditGenre()
        {
            var instructors = await _context.Genres.ToListAsync();
            return PartialView("~/Views/Genres/SelectedEdit.cshtml", instructors);

        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            return View( _context.Book.ToList());
               
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create(string? id)
        {
            if(id == null)
            {
                ViewBag.instructorId = null;
                return View();
            } else
            {
                ViewBag.instructorId = id;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book book, int instructorId)
        {
                book.Id = 0;
                var instructor = await _context.Genres.FindAsync(instructorId);
                string uniqueFileName = GetUploadedFileName(book);
                book.BookCoverUrl = uniqueFileName;
                book.Genre = instructor;
                _context.Add(book);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
        }

        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Book == null)
            {
                ViewBag.instructorId = null;
                return NotFound();
            }

            var book = await _context.Book.FindAsync(id);
            if (book == null)
            {
                ViewBag.instructorId = null;
                return NotFound();
            }
            ViewBag.instructorId = id;
            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Book book, int instructorId)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (true)
            {
                try
                {
                    string uniqueFileName = GetUploadedFileName(book);
                    var instructor = await _context.Genres.FindAsync(instructorId);
                    book.BookCoverUrl = uniqueFileName;
                    book.Genre = instructor;
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }
        }



        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Book.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            if (book.BookCoverUrl != null)
            {
                // delete the old image file from the server
                var imagePath = Path.Combine(webHost.WebRootPath, "resource/image/", book.BookCoverUrl);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _context.Book.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
          return (_context.Book?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private string GetUploadedFileName(Book book)
        {
            string uniqueFileName = null;

            if (book.BookPhoto != null)
            {
                string uploadsFolder = Path.Combine(webHost.WebRootPath, "resource/image");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + book.BookPhoto.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    book.BookPhoto.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        private bool IsImage(IFormFile file)
        {
            if (file.ContentType.Contains("image"))
            {
                return true;
            }

            string[] permittedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
            string fileExtension = Path.GetExtension(file.FileName);
            return permittedExtensions.Contains(fileExtension.ToLower());
        }

        

    }
}
