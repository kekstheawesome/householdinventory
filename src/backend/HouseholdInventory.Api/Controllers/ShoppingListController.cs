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
public class ShoppingListController(ApplicationDbContext dbContext, IAuditService auditService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> Get() => Ok(await dbContext.ShoppingListItems.AsNoTracking().ToListAsync());

    [HttpPost]
    public async Task<ActionResult> Create(ShoppingListItemRequest request)
    {
        var item = new ShoppingListItem { Name = request.Name, Quantity = request.Quantity, Unit = request.Unit, Notes = request.Notes, InventoryItemId = request.InventoryItemId };
        dbContext.ShoppingListItems.Add(item);
        await dbContext.SaveChangesAsync();
        await auditService.WriteAsync(AuditActionType.Create, nameof(ShoppingListItem), item.Id, "ShoppingList", $"Added {item.Name} to shopping list", null, item);
        return CreatedAtAction(nameof(Get), new { item.Id }, item);
    }

    [HttpPost("{id:guid}/complete")]
    public async Task<ActionResult> Complete(Guid id)
    {
        var item = await dbContext.ShoppingListItems.FirstOrDefaultAsync(x => x.Id == id);
        if (item is null) return NotFound();
        item.IsCompleted = true;
        await dbContext.SaveChangesAsync();
        return NoContent();
    }
}
