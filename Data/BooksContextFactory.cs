using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MyFirstApi.Data
{
    public class BooksContextFactory : IDesignTimeDbContextFactory<BooksContext>
    {
        public BooksContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BooksContext>();

            // Same connection string you put in appsettings.json
            optionsBuilder.UseSqlServer(
                "Server=localhost\\SQLEXPRESS;Database=BooksDb;Trusted_Connection=True;TrustServerCertificate=True;"
            );

            return new BooksContext(optionsBuilder.Options);
        }
    }
}
