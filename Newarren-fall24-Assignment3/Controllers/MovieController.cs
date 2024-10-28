using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newarren_fall24_Assignment3.Data;
using Newarren_fall24_Assignment3.Models;
using System.Diagnostics;

namespace Newarren_fall24_Assignment3.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Movies.ToListAsync());

        }
        public async Task<IActionResult> Create([Bind("Id,Name,Length,ReleaseYear,IMDBLink")] Movie movie)
        {

            if (ModelState.IsValid)
            {
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }


            return View(movie);
        }
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies.FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id) 
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null)
            {
                _context.Remove(movie);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> EditMenu(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("Id,Name,Length,ReleaseYear,IMDBLink,Poster")] Movie movie)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (movie == null)
            {
                return NotFound();
            }

            var photo = Request.Form.Files["Photo"]; // Get the uploaded photo
            if (ModelState.IsValid)
            {
                try
                {
                    if (photo != null && photo.Length > 0) // Check if a new photo was uploaded
                    {
                        using var memoryStream = new MemoryStream();
                        await photo.CopyToAsync(memoryStream);
                        movie.Poster = memoryStream.ToArray(); // Update the photo
                    }
                    else
                    {
                        //If no new photo, fetch the existing actor to retain the current photo
                        var existingMovie = await _context.Movies.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
                        movie.Poster = existingMovie.Poster; // Retain the existing photo
                    }

                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
