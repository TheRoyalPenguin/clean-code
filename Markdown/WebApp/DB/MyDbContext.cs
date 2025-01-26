using Microsoft.EntityFrameworkCore;
using WebApp.DB.Models;

namespace WebApp.DB;

public class MyDbContext : DbContext
{
    public DbSet<User> Users { get; set; } // позволяет выполнять операции с таблицей Users в базе данных
    public DbSet<Document> Documents { get; set; }
    public DbSet<DocumentPermission> DocumentPermissions { get; set; }
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // настройка уникального индекса для поля Email
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Document>()
            .HasOne(d => d.Owner)
            .WithMany()
            .HasForeignKey(d => d.OwnerId)
            .OnDelete(DeleteBehavior.Restrict); // документы не удалятся после удаления пользователя

        modelBuilder.Entity<DocumentPermission>()
            .HasKey(dp => new { dp.DocumentId, dp.UserId });

        modelBuilder.Entity<DocumentPermission>()
            .HasOne(dp => dp.Document)
            .WithMany(d => d.Permissions)
            .HasForeignKey(dp => dp.DocumentId)
            .OnDelete(DeleteBehavior.Cascade); // при удалении документа все связанные с ним DocumentPermission будут удалены
        
        modelBuilder.Entity<DocumentPermission>()
            .HasOne(dp => dp.User)
            .WithMany()
            .HasForeignKey(dp => dp.UserId)
            .OnDelete(DeleteBehavior.Restrict); // при удалении пользователя связанные с ним DocumentPermission не будут удалены
    }
}
