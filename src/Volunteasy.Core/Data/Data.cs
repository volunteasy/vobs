using EntityFramework.Exceptions.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Volunteasy.Core.Model;
using Volunteasy.Core.Services;

namespace Volunteasy.Core.Data;

public class Data : DbContext
{

    private readonly ILogger<Data> _log;
    public Data(DbContextOptions opt, ILogger<Data> log) : base(opt)
    {
        _log = log;
    }
    

    public DbSet<Address> Addresses { get; init; } = null!;
    
    public DbSet<User> Users { get; init; } = null!;
    
    public DbSet<Organization> Organizations { get; init; } = null!;



    public DbSet<Distribution> Distributions { get; init; } = null!;
    
    public DbSet<Membership> Memberships { get; init; } = null!;

    public DbSet<Resource> Resources { get; init; } = null!;
    
    public DbSet<StockMovement> StockMovements { get; init; } = null!;

    public DbSet<Benefit> Benefits { get; init; } = null!;

    public int? BenefitQueuePosition(long distributionId, long benefitId) =>
        Benefits
            .Where(b => b.DistributionId == distributionId && b.ClaimedAt == null)
            .OrderBy(b => b.Position ?? 0)
            .ToList()
            .Select((b, idx) => new { Pos = idx + 1, Id = b.Id })
            .SingleOrDefault(b => b.Id == benefitId)?.Pos;
    public DbSet<BenefitItem> BenefitItems { get; init; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(x =>
        {
            x.Property(u => u.Id).HasValueGenerator<IdValueGenerator>();
            x.HasIndex(u => u.Document).IsUnique();
        });
            
        modelBuilder.Entity<Address>(x =>
        {
            x.Property(a => a.Id).HasValueGenerator<IdValueGenerator>();
        });

        modelBuilder.Entity<Organization>(x =>
        {
            x.Property(o => o.Id).HasValueGenerator<IdValueGenerator>();
            x.HasIndex(c => c.Document).IsUnique();
            
            x.HasOne<Address>(o => o.Address).WithOne()
                .HasForeignKey<Organization>(o => o.AddressId);
        });

        modelBuilder.Entity<Membership>(x =>
        {
            x.HasKey(m => new { m.MemberId, m.OrganizationId });
            x.HasIndex(m => m.Role);
            x.HasIndex(m => m.Status);

            x.HasOne<Organization>().WithMany(o => o.Memberships)
                .HasForeignKey(m => m.OrganizationId).IsRequired();
            
            x.HasOne<User>().WithMany(u => u.Memberships)
                .HasForeignKey(m => m.MemberId).IsRequired();
        });
        
        modelBuilder.Entity<Resource>(x =>
        {
            x.Property(r => r.Id).HasValueGenerator<IdValueGenerator>();
            x.HasIndex(r => r.Name);

            x.HasOne<Organization>().WithMany(o => o.Resources)
                .HasForeignKey(r => r.OrganizationId).IsRequired();
        });
        
        modelBuilder.Entity<StockMovement>(x =>
        {
            x.Property(s => s.Id).HasValueGenerator<IdValueGenerator>();
            x.HasIndex(s => s.Type);
            x.HasIndex(s => s.Date);

            x.HasOne<Organization>().WithMany()
                .HasForeignKey(s => s.OrganizationId).IsRequired();
            
            x.HasOne<Resource>(s => s.Resource).WithMany()
                .HasForeignKey(s => s.ResourceId).IsRequired();
        });
        
        modelBuilder.Entity<Distribution>(x =>
        {
            x.Property(d => d.Id).HasValueGenerator<IdValueGenerator>();
            x.HasIndex(d => d.Name);

            x.HasOne<Organization>().WithMany(o => o.Distributions)
                .HasForeignKey(d => d.OrganizationId).IsRequired();
        });
        
        modelBuilder.Entity<Benefit>(x =>
        {
            x.Property(b => b.Id).HasValueGenerator<IdValueGenerator>();

            x.HasOne<Organization>().WithMany(o => o.Benefits)
                .HasForeignKey(b => b.OrganizationId).IsRequired();
            
            x.HasOne<Distribution>(b => b.Distribution).WithMany(d => d.Benefits)
                .HasForeignKey(b => b.DistributionId);
        });
        
        modelBuilder.Entity<BenefitItem>(x =>
        {
            x.HasKey(b => new { b.BenefitId, b.ResourceId });
            
            x.HasOne<Benefit>().WithMany(b => b.Items)
                .HasForeignKey(b => b.BenefitId).IsRequired();
            
            x.HasOne<Resource>(b => b.Resource).WithMany()
                .HasForeignKey(b => b.ResourceId).IsRequired();

            x.HasOne<StockMovement>(b => b.StockMovement).WithOne()
                .HasForeignKey<BenefitItem>(b => b.StockMovementId)
                .IsRequired();
        });

    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseExceptionProcessor();
    }
}