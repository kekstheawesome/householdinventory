using HouseholdInventory.Domain.Enums;

namespace HouseholdInventory.Application.DTOs;

public record InventoryItemRequest(
    string Name,
    string Description,
    InventoryCategory Category,
    decimal Quantity,
    string Unit,
    decimal MinimumStockThreshold,
    DateTime? ExpiryDateUtc,
    string Location);

public record QuantityUpdateRequest(decimal Amount, string Reason);

public record InventoryItemDto(
    Guid Id,
    string Name,
    string Description,
    InventoryCategory Category,
    decimal Quantity,
    string Unit,
    decimal MinimumStockThreshold,
    DateTime? ExpiryDateUtc,
    string Location,
    bool IsLowStock,
    bool IsOutOfStock);
