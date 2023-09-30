using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bemay.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
        public string? Title { get; set; }
        public List<BookCategory>? BookCategories { get; set; }
        
        [NotMapped]
        public List<int>? CategoryIds { get; set; }
        public int AuthorId { get; set; }
        [ForeignKey("AuthorId")]
        public Author? Author { get; set; }
        public double? Price { get; set; }
        public int? EditionYear { get; set; }
        public string? Language { get; set; }
        public int? PageCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

}
