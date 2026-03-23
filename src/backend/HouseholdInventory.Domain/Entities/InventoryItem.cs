using HouseholdInventory.Domain.Common;
using HouseholdInventory.Domain.Enums;

namespace HouseholdInventory.Domain.Entities;

public class InventoryItem : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public InventoryCategory Category { get; set; }
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = "pcs";
    public decimal MinimumStockThreshold { get; set; }
    public DateTime? ExpiryDateUtc { get; set; }
    public string Location { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
}
