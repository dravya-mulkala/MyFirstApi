// Book.cs
namespace MyFirstApi.Models
{
    public class Book
    {
        public int BookId { get; set; }
        public string? Title { get; set; }

        // Foreign Key (each Book belongs to one Publisher)
        public int PublisherId { get; set; }

        // Navigation property (Book → Publisher)
        public Publisher? Publisher { get; set; }
    }
}
