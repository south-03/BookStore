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

        // GET: Books
        public async Task<IActionResult> Index()
        {
            return View( _context.Book.ToList());
               /* _context.Book != null ?
                          View(await _context.Book.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Book'  is null.");*/
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
        public IActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Book book)
        {
                string uniqueFileName = GetUploadedFileName(book);
                book.BookCoverUrl = uniqueFileName;
                _context.Add(book);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
        }

        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Author,Description,Category,Price,TotalPage,CreatedOn,UpdatedOn,BookCoverUrl")] Book book, IFormFile BookPhoto)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (true)
            {
                try
                {
                    // Check if a new cover image file was uploaded
                    if (BookPhoto != null)
                    {
                        // Check if the uploaded file is an image
                        if (!IsImage(BookPhoto))
                        {
                            ModelState.AddModelError(string.Empty, "Please select a valid image file for the cover image.");
                            return View(book);
                        }


                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(BookPhoto.FileName);


                        string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "resource", "image");


                        if (!Directory.Exists(uploadPath))
                        {
                            Directory.CreateDirectory(uploadPath);
                        }


                        string filePath = Path.Combine(uploadPath, fileName);

                        if (!string.IsNullOrEmpty(book.BookCoverUrl))
                        {
                            string oldFilePath = Path.Combine(uploadPath, book.BookCoverUrl);
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }


                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await BookPhoto.CopyToAsync(stream);
                        }

                        book.BookCoverUrl = fileName;
                    }

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
