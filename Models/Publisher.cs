// Publisher.cs
using System.Collections.Generic;

namespace MyFirstApi.Models
{
    public class Publisher
    {
        public int PublisherId { get; set; }
        public string? Name { get; set; } = string.Empty;

        // Navigation property (One Publisher → Many Books)
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
