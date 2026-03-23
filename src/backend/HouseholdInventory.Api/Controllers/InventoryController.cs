using HouseholdInventory.Application.DTOs;
using HouseholdInventory.Application.Interfaces;
using HouseholdInventory.Domain.Entities;
using HouseholdInventory.Domain.Enums;
using HouseholdInventory.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HouseholdInventory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "HouseholdMember")]
public class InventoryController(ApplicationDbContext dbContext, IAuditService auditService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAll()
        => Ok(await dbContext.InventoryItems.AsNoTracking().Select(x => new InventoryItemDto(x.Id, x.Name, x.Description, x.Category, x.Quantity, x.Unit, x.MinimumStockThreshold, x.ExpiryDateUtc, x.Location, x.Quantity <= x.MinimumStockThreshold && x.Quantity > 0, x.Quantity <= 0)).ToListAsync());

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult> Create(InventoryItemRequest request)
    {
        var item = new InventoryItem
        {
            Name = request.Name,
            Description = request.Description,
            Category = request.Category,
            Quantity = request.Quantity,
            Unit = request.Unit,
            MinimumStockThreshold = request.MinimumStockThreshold,
            ExpiryDateUtc = request.ExpiryDateUtc,
            Location = request.Location
        };
        dbContext.InventoryItems.Add(item);
        await dbContext.SaveChangesAsync();
        await auditService.WriteAsync(AuditActionType.Create, nameof(InventoryItem), item.Id, item.Category.ToString(), $"Created inventory item {item.Name}", null, item);
        return CreatedAtAction(nameof(GetAll), new { item.Id }, item);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult> Update(Guid id, InventoryItemRequest request)
    {
        var item = await dbContext.InventoryItems.FirstOrDefaultAsync(x => x.Id == id);
        if (item is null) return NotFound();
        var oldValues = new { item.Name, item.Quantity, item.MinimumStockThreshold, item.Category };
        item.Name = request.Name; item.Description = request.Description; item.Category = request.Category; item.Quantity = request.Quantity; item.Unit = request.Unit; item.MinimumStockThreshold = request.MinimumStockThreshold; item.ExpiryDateUtc = request.ExpiryDateUtc; item.Location = request.Location; item.UpdatedAtUtc = DateTime.UtcNow;
        await dbContext.SaveChangesAsync();
        await auditService.WriteAsync(AuditActionType.Update, nameof(InventoryItem), item.Id, item.Category.ToString(), $"Updated inventory item {item.Name}", oldValues, item);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var item = await dbContext.InventoryItems.FirstOrDefaultAsync(x => x.Id == id);
        if (item is null) return NotFound();
        item.IsDeleted = true;
        await dbContext.SaveChangesAsync();
        await auditService.WriteAsync(AuditActionType.Delete, nameof(InventoryItem), item.Id, item.Category.ToString(), $"Deleted inventory item {item.Name}", item, null);
        return NoContent();
    }

    [HttpPost("{id:guid}/restock")]
    public async Task<ActionResult> Restock(Guid id, QuantityUpdateRequest request)
    {
        var item = await dbContext.InventoryItems.FirstOrDefaultAsync(x => x.Id == id);
        if (item is null) return NotFound();
        var oldQuantity = item.Quantity;
        item.Quantity += request.Amount;
        await dbContext.SaveChangesAsync();
        await auditService.WriteAsync(AuditActionType.Restock, nameof(InventoryItem), item.Id, item.Category.ToString(), $"Restocked {item.Name}", new { Quantity = oldQuantity }, new { item.Quantity, request.Reason });
        return NoContent();
    }

    [HttpPost("{id:guid}/consume")]
    public async Task<ActionResult> Consume(Guid id, QuantityUpdateRequest request)
    {
        var item = await dbContext.InventoryItems.FirstOrDefaultAsync(x => x.Id == id);
        if (item is null) return NotFound();
        var oldQuantity = item.Quantity;
        item.Quantity = Math.Max(0, item.Quantity - request.Amount);
        await dbContext.SaveChangesAsync();
        await auditService.WriteAsync(AuditActionType.Consume, nameof(InventoryItem), item.Id, item.Category.ToString(), $"Consumed {item.Name}", new { Quantity = oldQuantity }, new { item.Quantity, request.Reason });
        return NoContent();
    }
}
