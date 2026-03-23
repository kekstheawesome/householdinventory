using HouseholdInventory.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HouseholdInventory.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, Microsoft.AspNetCore.Identity.IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<ShoppingListItem> ShoppingListItems => Set<ShoppingListItem>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<InventoryItem>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<InventoryItem>().Property(x => x.Quantity).HasColumnType("decimal(18,2)");
        builder.Entity<InventoryItem>().Property(x => x.MinimumStockThreshold).HasColumnType("decimal(18,2)");
        builder.Entity<ShoppingListItem>().Property(x => x.Quantity).HasColumnType("decimal(18,2)");
    }
}
