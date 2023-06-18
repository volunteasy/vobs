using EntityFramework.Exceptions.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Volunteasy.Core.Model;

namespace Volunteasy.Core.Data;

public class Data : DbContext
{
    public Data(DbContextOptions opt) : base(opt) {}

    public DbSet<User> Users { get; init; } = null!;

    public DbSet<Organization> Organizations { get; init; } = null!;

    public DbSet<Membership> Memberships { get; set; } = null!;

    public DbSet<Address> Addresses { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(x =>
        {
            x.Property(u => u.Id).HasValueGenerator<IdValueGenerator>();
            x.HasIndex(c => c.Document).IsUnique();
        });
            

        modelBuilder.Entity<Organization>(x =>
        {
            x.Property(u => u.Id).HasValueGenerator<IdValueGenerator>();
            x.HasIndex(c => c.Document).IsUnique();
        });

        modelBuilder.Entity<Membership>(x =>
        {
            x.HasKey(m => new { m.MemberId, m.OrganizationId });
            x.HasIndex(m => m.Role);
            x.HasIndex(m => m.Status);
        });

    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseExceptionProcessor();
    }
}