using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newarren_fall24_Assignment3.Data;
using Newarren_fall24_Assignment3.Models;
using Newarren_fall24_Assignment3.Services;
using System.Diagnostics;
using VaderSharp2;

namespace Newarren_fall24_Assignment3.Controllers
{
    public class ActorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly OpenAIService _openAIService;

        public ActorsController(ApplicationDbContext context, OpenAIService OAIService)
        {
            _context = context;
            _openAIService = OAIService;
        }
        //private readonly OpenAI _openAIService;

        public async Task<IActionResult> Index()
        {
            return View(await _context.Actors.ToListAsync());
        }
        public async Task<IActionResult> Create([Bind("Id,Name,Gender,Age,IMDBLink, Photo")] Actor actor)
        {

            if (ModelState.IsValid)
            {
                var sentiments = new List<string>();
                var tweets = await _openAIService.GenerateActorTweetsAsync(actor.Name);
                actor.Tweets = tweets;
                var photo = Request.Form.Files["Photo"];
                //sentemental analysis
                SentimentIntensityAnalyzer analyzer = new SentimentIntensityAnalyzer();
                foreach (var tweet in tweets)
                {
                    var score = 0;
                    var result = analyzer.PolarityScores(tweet);
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
                if (photo != null && photo.Length > 0)
                {
                    using var memoryStream = new MemoryStream();
                    await photo.CopyToAsync(memoryStream);
                    actor.Photo = memoryStream.ToArray();
                }

                actor.Sentiments = sentiments;
                actor.Movies = await _openAIService.GenerateMovieListAsync(actor.Name);
                _context.Add(actor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(actor);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
               return NotFound();
            }

            var actor = await _context.Actors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (actor == null)
            {
                return NotFound();
            }


            return View(actor);
        }
        // GET: Actors/Delete/5
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _context.Actors.FirstOrDefaultAsync(m => m.Id == id);
            if (actor == null)
            {
                return NotFound();
            }

            return View(actor);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var actor = await _context.Actors.FindAsync(id);
            if (actor != null)
            {
                _context.Actors.Remove(actor);
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
            var actor = await _context.Actors.FindAsync(id);
            if (actor == null)
            {
                return NotFound();
            }
            return View(actor);
        }
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("Id,Name,Gender,Age,IMDBLink,Photo")] Actor actor)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (actor == null)
            {
                return NotFound();
            }

            var photo = Request.Form.Files["Photo"]; // Get the uploaded photo
            if (ModelState.IsValid)
            {
                try
                {
                    var existingActor = await _context.Actors.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
                    if (photo != null && photo.Length > 0) // Check if a new photo was uploaded
                    {
                        using var memoryStream = new MemoryStream();
                        await photo.CopyToAsync(memoryStream);
                        actor.Photo = memoryStream.ToArray(); // Update the photo
                    }
                    else
                    {
                        actor.Photo = existingActor.Photo; // Retain the existing photo
                    }
                    actor.Movies = existingActor.Movies;
                    actor.Sentiments = existingActor.Sentiments;
                    actor.Tweets = existingActor.Tweets;
                    _context.Update(actor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(actor);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
