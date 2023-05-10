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
        public double Price { get; set; }
        public int TotalPage { get; set; }
        public string BookCoverUrl { get; set; }

        [NotMapped]
        [Display(Name = "Book Photo")]
        public IFormFile? BookPhoto { get; set; }

        [Required]
        public int GenreId { get; set; }
        public virtual Genre? Genre { get; set; }
        public List<OrderDetail> OrderDetail { get; set; }
        public List<CartDetail> CartDetail { get; set; }

        [NotMapped]
        public string GenreName { get; set; }
    }
}
