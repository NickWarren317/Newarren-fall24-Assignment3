using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newarren_fall24_Assignment3.Data;
using Newarren_fall24_Assignment3.Models;
using Newarren_fall24_Assignment3.Services;
using System.Diagnostics;

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
                actor.Tweets = await _openAIService.GenerateActorTweetsAsync(actor.Name);
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
                    if (photo != null && photo.Length > 0) // Check if a new photo was uploaded
                    {
                        using var memoryStream = new MemoryStream();
                        await photo.CopyToAsync(memoryStream);
                        actor.Photo = memoryStream.ToArray(); // Update the photo
                    }
                    else
                    {
                        //If no new photo, fetch the existing actor to retain the current photo
                        var existingActor = await _context.Actors.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
                        actor.Photo = existingActor.Photo; // Retain the existing photo
                    }

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
