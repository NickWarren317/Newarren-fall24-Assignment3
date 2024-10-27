using System.ComponentModel.DataAnnotations;

namespace Newarren_fall24_Assignment3.Models
{
    public class Actor
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public string? Gender { get; set; }
        public int? Age { get; set; }
        public string? IMDBLink { get; set; }
        public byte[]? Photo { get; set; }
    }
}
