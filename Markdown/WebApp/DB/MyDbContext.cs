using Microsoft.EntityFrameworkCore;
using WebApp.DB.Models;

namespace WebApp.DB;

public class MyDbContext : DbContext
{
    public DbSet<User> Users { get; set; } // позволяет выполнять операции с таблицей Users в базе данных
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }
}
