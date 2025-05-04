using System.Data.Entity; // Make sure this is imported
using StepInStyle.Models; // Assuming your models are here

namespace StepInStyle.Models
{
    public class StepInStyleContext : DbContext
    {
        public StepInStyleContext() : base("name=StepInStyleContext")
        {
        }

        // Add your tables here, for example:
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }

        // Add more DbSets as per your design
    }
}
