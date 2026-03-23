using HouseholdInventory.Application.DTOs;
using HouseholdInventory.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HouseholdInventory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "AdminOnly")]
public class AuditController(ApplicationDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> Get([FromQuery] AuditLogFilterDto filter)
    {
        var query = dbContext.AuditLogs.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(filter.UserEmail)) query = query.Where(x => x.UserEmail == filter.UserEmail);
        if (filter.ItemId.HasValue) query = query.Where(x => x.EntityId == filter.ItemId);
        if (!string.IsNullOrWhiteSpace(filter.Category)) query = query.Where(x => x.Category == filter.Category);
        if (!string.IsNullOrWhiteSpace(filter.Action)) query = query.Where(x => x.Action.ToString() == filter.Action);
        if (filter.FromUtc.HasValue) query = query.Where(x => x.TimestampUtc >= filter.FromUtc);
        if (filter.ToUtc.HasValue) query = query.Where(x => x.TimestampUtc <= filter.ToUtc);
        return Ok(await query.OrderByDescending(x => x.TimestampUtc).ToListAsync());
    }
}
