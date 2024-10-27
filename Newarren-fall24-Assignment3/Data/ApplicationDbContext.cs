using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newarren_fall24_Assignment3.Models;

namespace Newarren_fall24_Assignment3.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Movie> Movies {get; set;}
        public DbSet<Actor>  Actors {get; set;} 
    }
}
