using Microsoft.EntityFrameworkCore;
using Trading.Domain.Entities;

namespace Trading.Infrastructure.Persistence;

public sealed class TradingDbContext(DbContextOptions<TradingDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Trade> Trades => Set<Trade>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("orders");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Asset).HasMaxLength(20).IsRequired();
            entity.Property(x => x.Type).HasConversion<string>().IsRequired();
            entity.Property(x => x.Status).HasConversion<string>().IsRequired();
            entity.Property(x => x.Price).HasPrecision(18, 8).IsRequired();
            entity.HasIndex(x => new { x.Asset, x.Status, x.Price });
            entity.HasIndex(x => x.CreatedAt);
        });

        modelBuilder.Entity<Trade>(entity =>
        {
            entity.ToTable("trades");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Asset).HasMaxLength(20).IsRequired();
            entity.Property(x => x.ExecutionPrice).HasPrecision(18, 8).IsRequired();
            entity.HasIndex(x => new { x.Asset, x.ExecutedAt });
            entity.HasIndex(x => x.BuyOrderId);
            entity.HasIndex(x => x.SellOrderId);
            entity.HasOne<Order>().WithMany().HasForeignKey(x => x.BuyOrderId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<Order>().WithMany().HasForeignKey(x => x.SellOrderId).OnDelete(DeleteBehavior.Restrict);
        });
    }
}
