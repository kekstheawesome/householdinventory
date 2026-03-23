using HouseholdInventory.Application.DTOs;
using HouseholdInventory.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HouseholdInventory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "HouseholdMember")]
public class DashboardController(ApplicationDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<DashboardDto>> Get()
    {
        var items = dbContext.InventoryItems.AsQueryable();
        var dto = new DashboardDto(
            await items.CountAsync(),
            await items.CountAsync(x => x.Quantity <= x.MinimumStockThreshold && x.Quantity > 0),
            await items.CountAsync(x => x.Quantity <= 0),
            await items.CountAsync(x => x.ExpiryDateUtc != null && x.ExpiryDateUtc <= DateTime.UtcNow.AddDays(7)),
            await dbContext.ShoppingListItems.CountAsync(x => !x.IsCompleted),
            await dbContext.AuditLogs.OrderByDescending(x => x.TimestampUtc).Take(10).Select(x => new { x.Summary, x.Action, x.UserEmail, x.TimestampUtc }).ToListAsync());
        return Ok(dto);
    }
}
