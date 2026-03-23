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

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetById(Guid id)
    {
        var item = await dbContext.ShoppingListItems.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (item is null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult> Create(ShoppingListItemRequest request)
    {
        var item = new ShoppingListItem { Name = request.Name, Quantity = request.Quantity, Unit = request.Unit, Notes = request.Notes, InventoryItemId = request.InventoryItemId };
        dbContext.ShoppingListItems.Add(item);
        await auditService.WriteAsync(AuditActionType.Create, nameof(ShoppingListItem), item.Id, "ShoppingList", $"Added {item.Name} to shopping list", null, item);
        await dbContext.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPost("{id:guid}/complete")]
    public async Task<ActionResult> Complete(Guid id)
    {
        var item = await dbContext.ShoppingListItems.FirstOrDefaultAsync(x => x.Id == id);
        if (item is null) return NotFound();

        var previousIsCompleted = item.IsCompleted;
        item.IsCompleted = true;
        await auditService.WriteAsync(
            AuditActionType.Update,
            nameof(ShoppingListItem),
            item.Id,
            "ShoppingList",
            $"Marked {item.Name} as completed",
            new { IsCompleted = previousIsCompleted },
            new { item.IsCompleted });
        await dbContext.SaveChangesAsync();
        return NoContent();
    }
}
