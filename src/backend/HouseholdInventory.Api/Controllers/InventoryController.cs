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

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<InventoryItemDto>> GetById(Guid id)
    {
        var item = await dbContext.InventoryItems.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
        if (item is null) return NotFound();
        return Ok(new InventoryItemDto(item.Id, item.Name, item.Description, item.Category, item.Quantity, item.Unit, item.MinimumStockThreshold, item.ExpiryDateUtc, item.Location, item.Quantity <= item.MinimumStockThreshold && item.Quantity > 0, item.Quantity <= 0));
    }

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
        await auditService.WriteAsync(AuditActionType.Create, nameof(InventoryItem), item.Id, item.Category.ToString(), $"Created inventory item {item.Name}", null, item);
        await dbContext.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult> Update(Guid id, InventoryItemRequest request)
    {
        var item = await dbContext.InventoryItems.FirstOrDefaultAsync(x => x.Id == id);
        if (item is null) return NotFound();
        var oldValues = new { item.Name, item.Quantity, item.MinimumStockThreshold, item.Category };
        item.Name = request.Name; item.Description = request.Description; item.Category = request.Category; item.Quantity = request.Quantity; item.Unit = request.Unit; item.MinimumStockThreshold = request.MinimumStockThreshold; item.ExpiryDateUtc = request.ExpiryDateUtc; item.Location = request.Location; item.UpdatedAtUtc = DateTime.UtcNow;
        await auditService.WriteAsync(AuditActionType.Update, nameof(InventoryItem), item.Id, item.Category.ToString(), $"Updated inventory item {item.Name}", oldValues, item);
        await dbContext.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var item = await dbContext.InventoryItems.FirstOrDefaultAsync(x => x.Id == id);
        if (item is null) return NotFound();
        var oldValues = new
        {
            item.Name,
            item.Description,
            item.Category,
            item.Quantity,
            item.Unit,
            item.MinimumStockThreshold,
            item.ExpiryDateUtc,
            item.Location,
            item.IsDeleted
        };
        item.IsDeleted = true;
        await auditService.WriteAsync(AuditActionType.Delete, nameof(InventoryItem), item.Id, item.Category.ToString(), $"Deleted inventory item {item.Name}", oldValues, null);
        await dbContext.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{id:guid}/restock")]
    public async Task<ActionResult> Restock(Guid id, QuantityUpdateRequest request)
    {
        if (request.Amount <= 0) return BadRequest(new { Error = "Amount must be greater than zero." });
        var item = await dbContext.InventoryItems.FirstOrDefaultAsync(x => x.Id == id);
        if (item is null) return NotFound();
        var oldQuantity = item.Quantity;
        item.Quantity += request.Amount;
        await auditService.WriteAsync(AuditActionType.Restock, nameof(InventoryItem), item.Id, item.Category.ToString(), $"Restocked {item.Name}", new { Quantity = oldQuantity }, new { item.Quantity, request.Reason });
        await dbContext.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{id:guid}/consume")]
    public async Task<ActionResult> Consume(Guid id, QuantityUpdateRequest request)
    {
        if (request.Amount <= 0) return BadRequest(new { Error = "Amount must be greater than zero." });
        var item = await dbContext.InventoryItems.FirstOrDefaultAsync(x => x.Id == id);
        if (item is null) return NotFound();
        var oldQuantity = item.Quantity;
        item.Quantity = Math.Max(0, item.Quantity - request.Amount);
        await auditService.WriteAsync(AuditActionType.Consume, nameof(InventoryItem), item.Id, item.Category.ToString(), $"Consumed {item.Name}", new { Quantity = oldQuantity }, new { item.Quantity, request.Reason });
        await dbContext.SaveChangesAsync();
        return NoContent();
    }
}
