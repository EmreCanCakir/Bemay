using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bemay.Models;
using Microsoft.AspNetCore.Authorization;

namespace Bemay.Controllers
{
    public class BooksController : Controller
    {
        private readonly AppDbContext _context;

        public BooksController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Books.Include(b => b.Author).Include(b => b.BookCategories).ThenInclude(bc => bc.Category);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.BookCategories) // Include the associated categories
                .ThenInclude(bc => bc.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        [Authorize(Policy = "AdminPolicy")]
        public IActionResult Create()
        {
            ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "Id");

            ViewData["CategoryList"] = new SelectList(_context.Categories, "Id", "CategoryName");

            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Policy = "AdminPolicy")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,AuthorId,Price,EditionYear,Language,PageCount, CategoryIds")] Book book, List<int> CategoryIds)
        {
            if (ModelState.IsValid)
            {
                book.CreatedAt = DateTime.Now;
                book.UpdatedAt = DateTime.Now;

                _context.Add(book);
                await _context.SaveChangesAsync();

                if (CategoryIds != null)
                {
                    foreach (int categoryId in CategoryIds)
                    {
                        var bookCategory = new BookCategory
                        {
                            BookId = book.Id,
                            CategoryId = categoryId
                        };
                        _context.Add(bookCategory);
                    }
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "Id", book.AuthorId);
            ViewData["CategoryList"] = new MultiSelectList(_context.Categories, "Id", "CategoryName", book.CategoryIds);

            return View(book);
        }

        // GET: Books/Edit/5
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            // Load the selected categories for the book
            var selectedCategories = await _context.BookCategories
                .Where(bc => bc.BookId == book.Id)
                .Select(bc => bc.CategoryId)
                .ToListAsync();

            ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "Id", book.AuthorId);

            ViewData["CategoryList"] = new MultiSelectList(_context.Categories, "Id", "CategoryName", selectedCategories);

            // Assign the selected categories to the CategoryIds property of the book
            book.CategoryIds = selectedCategories;

            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Policy = "AdminPolicy")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,AuthorId,Price,EditionYear,Language,PageCount, CategoryIds")] Book book, List<int> CategoryIds)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {                    
                    book.CreatedAt = DateTime.Now;
                    book.UpdatedAt = DateTime.Now;

                    // Update the book properties
                    _context.Update(book);

                    // Update the selected categories for the book
                    var existingCategories = await _context.BookCategories
                        .Where(bc => bc.BookId == book.Id)
                        .ToListAsync();

                    _context.RemoveRange(existingCategories);

                    if (CategoryIds != null)
                    {
                        foreach (int categoryId in CategoryIds)
                        {
                            var bookCategory = new BookCategory
                            {
                                BookId = book.Id,
                                CategoryId = categoryId
                            };
                            _context.Add(bookCategory);
                        }
                    }

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

            ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "Id", book.AuthorId);
            ViewData["CategoryList"] = new MultiSelectList(_context.Categories, "Id", "CategoryName", book.CategoryIds);

            return View(book);
        }

        // GET: Books/Delete/5
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Author)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Policy = "AdminPolicy")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Books == null)
            {
                return Problem("Entity set 'AppDbContext.Books'  is null.");
            }
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
          return (_context.Books?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
