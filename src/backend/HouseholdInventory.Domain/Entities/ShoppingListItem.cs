using HouseholdInventory.Domain.Common;

namespace HouseholdInventory.Domain.Entities;

public class ShoppingListItem : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = "pcs";
    public bool IsCompleted { get; set; }
    public string Notes { get; set; } = string.Empty;
    public Guid? InventoryItemId { get; set; }
    public InventoryItem? InventoryItem { get; set; }
}
