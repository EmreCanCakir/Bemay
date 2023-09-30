using System.ComponentModel.DataAnnotations;

namespace Bemay.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public string? CategoryName { get; set; }
        public List<BookCategory>? BookCategories { get; set; }
    }
}
