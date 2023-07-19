using EntityFramework.Exceptions.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Volunteasy.Core.DTOs;
using Volunteasy.Core.Enums;
using Volunteasy.Core.Model;
using Volunteasy.Core.Services;

namespace Volunteasy.Core.Data;

public class Data : DbContext
{
    public Data(DbContextOptions opt) : base(opt) { }

    public DbSet<Address> Addresses { get; init; } = null!;

    public DbSet<User> Users { get; init; } = null!;

    public DbSet<Organization> Organizations { get; init; } = null!;

    public IQueryable<OrganizationDetails> OrganizationDetails(long user) =>
        Organizations
            .AsQueryable()
            .Select(o => new OrganizationDetails
            {
                Id = o.Id,
                Address = o.Address ?? new(),
                Name = o.Name ?? string.Empty,
                PhoneNumber = o.PhoneNumber ?? string.Empty,
                Slug = o.Slug ?? "",
                Stats = new OrganizationStats
                {
                    AssistedPeopleCount = Memberships
                        .Count(x => x.OrganizationId == o.Id && x.Role == MembershipRole.Assisted),
                    NextDistributionsCount = Distributions.Count(x =>
                        x.OrganizationId == o.Id && x.StartsAt >= DateTime.Now.ToUniversalTime()),
                },
                Membership = Memberships
                    .Where(m => m.MemberId == user && m.OrganizationId == o.Id)
                    .Select(m => new MembershipStats
                    {
                        Role = m.Role,
                        MemberSince = m.MemberSince,
                        Status = m.Status,
                        BenefitsReceived = Benefits
                            .Count(b =>
                                b.OrganizationId == o.Id && b.AssistedId == m.MemberId && b.ClaimedAt != null)
                    }).SingleOrDefault()
            });


    public DbSet<Distribution> Distributions { get; init; } = null!;
    
    public IQueryable<DistributionDto> DistributionDetails(long user) =>
        Distributions
            .AsQueryable()
            .Select(d => new DistributionDto
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                StartsAt = d.StartsAt,
                EndsAt = d.EndsAt,
                MaxBenefits = d.MaxBenefits,
                Canceled = d.Canceled,
                OrganizationId = d.OrganizationId,
                RemainingBenefits = d.MaxBenefits - Benefits.Count(b => b.DistributionId == d.Id),
                Stats = new DistributionStats
                {
                    BenefitsToClaim = Benefits.Count(b => b.DistributionId == d.Id && b.ClaimedAt == null && b.RevokedReason == null),
                    ClaimedBenefits = Benefits.Count(b => b.DistributionId == d.Id && b.ClaimedAt == null && b.RevokedReason == null)
                },
                Benefit = Benefits
                    .Where(b => b.DistributionId == d.Id && b.AssistedId == user)
                    .Select(b => new BenefitStats
                    {
                        BenefitId = b.Id,
                        ClaimedAt = b.ClaimedAt,
                        RevokedReason = b.RevokedReason
                    }).SingleOrDefault()
            });

    public DbSet<Membership> Memberships { get; init; } = null!;

    public DbSet<Resource> Resources { get; init; } = null!;

    public DbSet<StockMovement> StockMovements { get; init; } = null!;

    public DbSet<Benefit> Benefits { get; init; } = null!;

    public DbSet<Beneficiary> Beneficiaries { get; init; } = null!;

    public int? BenefitQueuePosition(long distributionId, long benefitId) =>
        Benefits
            .Where(b => b.DistributionId == distributionId && b.ClaimedAt == null && b.RevokedReason == null)
            .OrderBy(b => b.Position ?? 0)
            .ToList()
            .Select((b, idx) => new { Pos = idx + 1, b.Id })
            .SingleOrDefault(b => b.Id == benefitId)?.Pos;

    public DbSet<BenefitItem> BenefitItems { get; init; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(x =>
        {
            x.Property(u => u.Id).HasValueGenerator<IdValueGenerator>();
            x.HasIndex(u => u.Document).IsUnique();
        });

        modelBuilder.Entity<Address>(x => { x.Property(a => a.Id).HasValueGenerator<IdValueGenerator>(); });

        modelBuilder.Entity<Organization>(x =>
        {
            x.Property(o => o.Id).HasValueGenerator<IdValueGenerator>();
            x.HasIndex(c => c.Document).IsUnique();
            x.HasIndex(c => c.Slug).IsUnique();

            x.HasOne<Address>(o => o.Address).WithOne()
                .HasForeignKey<Organization>(o => o.AddressId);

            // Organization children
            
            x.HasMany<Beneficiary>(o => o.Beneficiaries).WithOne()
                .HasForeignKey(b => b.OrganizationId).IsRequired();

            x.HasMany<Membership>(o => o.Memberships).WithOne()
                .HasForeignKey(m => m.OrganizationId).IsRequired();
            
            x.HasMany<Resource>(o => o.Resources).WithOne()
                .HasForeignKey(r => r.OrganizationId).IsRequired();
            
            x.HasMany<StockMovement>().WithOne()
                .HasForeignKey(s => s.OrganizationId).IsRequired();

            x.HasMany<Distribution>(o => o.Distributions).WithOne()
                .HasForeignKey(d => d.OrganizationId).IsRequired();
            
            x.HasMany<Benefit>(o => o.Benefits).WithOne()
                .HasForeignKey(d => d.OrganizationId).IsRequired();
            
            x.HasMany<BenefitItem>().WithOne()
                .HasForeignKey(d => d.OrganizationId).IsRequired();

        });

        modelBuilder.Entity<Membership>(x =>
        {
            x.HasKey(m => new { m.MemberId, m.OrganizationId });
            x.HasIndex(m => m.Role);
            x.HasIndex(m => m.Status);

            x.HasOne<User>().WithMany(u => u.Memberships)
                .HasForeignKey(m => m.MemberId).IsRequired();
        });

        modelBuilder.Entity<Resource>(x =>
        {
            x.Property(r => r.Id).HasValueGenerator<IdValueGenerator>();
            x.HasIndex(r => r.Name);
        });

        modelBuilder.Entity<StockMovement>(x =>
        {
            x.Property(s => s.Id).HasValueGenerator<IdValueGenerator>();
            x.HasIndex(s => s.Type);
            x.HasIndex(s => s.Date);

            x.HasOne<Resource>(s => s.Resource).WithMany()
                .HasForeignKey(s => s.ResourceId).IsRequired();
        });

        modelBuilder.Entity<Distribution>(x =>
        {
            x.Property(d => d.Id).HasValueGenerator<IdValueGenerator>();
            x.HasIndex(d => d.Name);
        });

        modelBuilder.Entity<Benefit>(x =>
        {
            x.Property(b => b.Id).HasValueGenerator<IdValueGenerator>();

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
        
        modelBuilder.Entity<Beneficiary>(x =>
        {
            x.Property(b => b.Id).HasValueGenerator<IdValueGenerator>();

            x.HasIndex(b => new { b.Document, b.BirthDate, b.OrganizationId })
                .IsUnique();

            x.HasOne<Address>(b => b.Address).WithOne()
                .HasForeignKey<Beneficiary>(b => b.AddressId);

            x.HasMany<Benefit>(b => b.Benefits)
                .WithOne().HasForeignKey(b => b.AssistedId);
        });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseExceptionProcessor();
    }
}