using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MVCTemplate.Models; 

namespace MVCtemplate.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categorys { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Category>().HasData(
                new Category { IdCategory = 1, NameCategory = "C1", CodeCategory = "1C"},
                new Category { IdCategory = 2, NameCategory = "C2", CodeCategory = "2C"},
                new Category { IdCategory = 3, NameCategory = "C3", CodeCategory = "3C"}
            
            );

        }
    }
}
