using Microsoft.EntityFrameworkCore;
using Volunteasy.Core.Model;

namespace Volunteasy.Core.Data;

public class Data : DbContext
{
    public Data(DbContextOptions opt) : base(opt) {}

    public DbSet<User> Users { get; init; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(c => c.Document).IsUnique();
    }
}