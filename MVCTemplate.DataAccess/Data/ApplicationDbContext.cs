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
        public DbSet<Person> Persons { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Report> Reports { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            /*
            builder.Entity<Category>().HasData(
                new Category { IdCategory = 1, NameCategory = "C11", CodeCategory = "11C"},
                new Category { IdCategory = 2, NameCategory = "C22", CodeCategory = "22C"},
                new Category { IdCategory = 3, NameCategory = "C23", CodeCategory = "33C"}
            
            );

            builder.Entity<Person>().HasData(
                new Person { Id = 1, Name = "Name1", Position = "E1", CategoryId = 1 },
                new Person { Id = 2, Name = "Name2", Position = "E2", CategoryId = 1 },
                new Person { Id = 3, Name = "Name3", Position = "E3", CategoryId = 1 }

            );
            */
        }
    }
}
