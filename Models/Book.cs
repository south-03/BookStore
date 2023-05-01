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
        public string Price { get; set; }
        public int TotalPage { get; set; }
        private DateTime? createdOn;
        [DataType(DataType.Date)]
        public DateTime CreatedOn
        {
            get { return createdOn ?? DateTime.Now; }
            set { createdOn = value; }
        }
        private DateTime? updatedOn;
        [DataType(DataType.Date)]
        public DateTime UpdatedOn
        {
            get { return updatedOn ?? DateTime.Now; }
            set { updatedOn = value; }
        }
        public string BookCoverUrl { get; set; }

        [NotMapped]
        [Display(Name = "Book Photo")]
        public IFormFile? BookPhoto { get; set; }
    }
}
