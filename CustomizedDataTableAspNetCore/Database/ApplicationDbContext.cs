using CustomizedDataTableAspNetCore.Database.Entities;
using CustomizedDataTableAspNetCore.Database.Seeders;
using Microsoft.EntityFrameworkCore;

namespace CustomizedDataTableAspNetCore.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var usersList = UsersTableSeeder.GenerateUsers();
            modelBuilder.Entity<User>().HasData(usersList);
        }
        public DbSet<User> Users { get; set; }

      
    }
}
