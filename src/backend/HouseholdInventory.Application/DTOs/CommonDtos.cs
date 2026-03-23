namespace HouseholdInventory.Application.DTOs;

public record ShoppingListItemRequest(string Name, decimal Quantity, string Unit, string Notes, Guid? InventoryItemId);
public record ChatRequest(string Question);
public record ChatResponse(string Answer, object Context);
public record DashboardDto(int TotalItems, int LowStockCount, int OutOfStockCount, int ExpiringSoonCount, int ShoppingListCount, IEnumerable<object> RecentActivity);
public record AuditLogFilterDto(string? UserEmail, Guid? ItemId, string? Category, string? Action, DateTime? FromUtc, DateTime? ToUtc);
