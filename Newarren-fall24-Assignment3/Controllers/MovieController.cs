
using Microsoft.AspNetCore.Mvc;
using Newarren_fall24_Assignment3.Models;
using Newarren_fall24_Assignment3.Data;
using Newarren_fall24_Assignment3.Data.Migrations;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Newarren_fall24_Assignment3.Controllers;

public class MovieController : Controller
{
    private readonly ApplicationDbContext _context;

    public MovieController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Actions will go here

    // GET: Movies
    public IActionResult Index()
    {
        var movies = _context.Movies.ToList();
        return View(movies);
    }

    // GET: Movies/Details/5
    public IActionResult Details(int id)
    {   
        var movie = _context.Movies.Find(id);
        if (movie == null)
        {
            return NotFound();
        }
        return View(movie);
    }

    // GET: Movies/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Movies/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Movie movie)
    {
        if (ModelState.IsValid)
        {
            _context.Add(movie);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        return View(movie);
    }

    // GET: Movies/Edit/id
    public IActionResult Edit(int id)
    {
        var movie = _context.Movies.Find(id);
        if (movie == null)
        {
            return NotFound();
        }
        return View(movie);
    }

    // POST: Movies/Edit/id
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Movie movie)
    {
        if (id != movie.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(movie);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Movies.Any(e => e.Id == movie.Id))
                {
                    return NotFound();
                }
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(movie);
    }

    // GET: Movies/Delete/id
    public IActionResult Delete(int id)
    {
        var movie = _context.Movies.Find(id);
        if (movie == null)
        {
            return NotFound();
        }
        return View(movie);
    }

    // POST: Movies/Delete/id
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var movie = _context.Movies.Find(id);
        if(movie == null)
        {
            return NotFound();
        }
        _context.Movies.Remove(movie);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }
}
