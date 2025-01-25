using Microsoft.EntityFrameworkCore;
using WebApp.DB.Models;

namespace WebApp.DB;

public class MyDbContext : DbContext
{
    public DbSet<User> Users { get; set; } // позволяет выполнять операции с таблицей Users в базе данных
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // настройка уникального индекса для поля Email
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }
}
