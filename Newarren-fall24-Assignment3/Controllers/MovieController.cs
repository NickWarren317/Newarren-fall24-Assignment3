using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newarren_fall24_Assignment3.Data;
using Newarren_fall24_Assignment3.Models;
using Newarren_fall24_Assignment3.Services;
using System.Diagnostics;
using VaderSharp2;

namespace Newarren_fall24_Assignment3.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly OpenAIService _openAIService;

        public MoviesController(ApplicationDbContext context, OpenAIService OAIService)
        {
            _context = context;
            _openAIService = OAIService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Movies.ToListAsync());

        }
        public async Task<IActionResult> Create([Bind("Id,Name,Length,ReleaseYear,IMDBLink, Poster")] Movie movie)
        {

            if (ModelState.IsValid)
            {
                movie.Reviews = await _openAIService.GenerateMovieReviewsAsync(movie.Name);
                var photo = Request.Form.Files["Poster"];
                var sentiments = new List<String>();
                SentimentIntensityAnalyzer analyzer = new SentimentIntensityAnalyzer();
                if (photo != null && photo.Length > 0)
                {
                    using var memoryStream = new MemoryStream();
                    await photo.CopyToAsync(memoryStream);
                    movie.Poster = memoryStream.ToArray();
                }

                foreach (var review in movie.Reviews)
                {
                    var score = 0;
                    var result = analyzer.PolarityScores(review);
                    string sentiment;

                    if (result.Compound >= 0.05)
                    {
                        score++;
                        sentiment = "Good";
                    }
                    else if (result.Compound <= -0.05)
                    {
                        score--;
                        sentiment = "Bad";
                    }
                    else
                    {
                        sentiment = "Neutral";
                    }

                    sentiments.Add(sentiment);
                }

                movie.Sentiments = sentiments;
                movie.Actors = await _openAIService.GenerateActorListAsync(movie.Name);
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

            var photo = Request.Form.Files["Photo"];
            if (ModelState.IsValid)
            {
                try
                {
                    var existingMovie = await _context.Movies.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
                    if (photo != null && photo.Length > 0)
                    {
                        using var memoryStream = new MemoryStream();
                        await photo.CopyToAsync(memoryStream);
                        movie.Poster = memoryStream.ToArray(); // Update the photo
                    }
                    else
                    {
                        movie.Poster = existingMovie.Poster; // Retain the existing photo
                    }
                    //readd information
                    movie.Sentiments = existingMovie.Sentiments;
                    movie.Reviews = existingMovie.Reviews;
                    movie.Actors = existingMovie.Actors;
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
