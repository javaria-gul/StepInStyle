using System.Data.Entity;

namespace StepInStyle.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("StepInStyleContext")
        {
            // Optional: Disable proxy creation if not using lazy loading
            this.Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

    }
}
