using System.ComponentModel.DataAnnotations;

namespace Newarren_fall24_Assignment3.Models
{
    public class Movie
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public string? Length { get; set; }
        public int? ReleaseYear { get; set; }
        public string? IMDBLink { get; set; }
        public byte[]? Poster { get; set; }

        public List<string> Reviews { get; set; }
    }
}
