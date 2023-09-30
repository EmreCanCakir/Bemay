using System.ComponentModel.DataAnnotations;

namespace Bemay.Models
{
    public class Author
    {
        [Key]
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? BirthYear { get; set; }
    }

}
