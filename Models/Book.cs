using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public int TotalPage { get; set; }
        public string CoverImagePath { get; set; }
        public string CoverImageName { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        [NotMapped]
        [DisplayName("Upload image")]
        public IFormFile CoverImageFile { get; set; }
    }
}
