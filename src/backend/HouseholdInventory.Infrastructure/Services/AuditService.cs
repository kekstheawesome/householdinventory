using System.Text.Json;
using HouseholdInventory.Application.Interfaces;
using HouseholdInventory.Domain.Entities;
using HouseholdInventory.Domain.Enums;
using HouseholdInventory.Infrastructure.Persistence;

namespace HouseholdInventory.Infrastructure.Services;

public class AuditService(ApplicationDbContext dbContext, ICurrentUserService currentUserService) : IAuditService
{
    public Task WriteAsync(AuditActionType action, string entityName, Guid? entityId, string category, string summary, object? oldValues, object? newValues, CancellationToken cancellationToken = default)
    {
        dbContext.AuditLogs.Add(new AuditLog
        {
            Action = action,
            EntityName = entityName,
            EntityId = entityId,
            Category = category,
            Summary = summary,
            UserId = currentUserService.UserId,
            UserEmail = currentUserService.Email,
            OldValuesJson = JsonSerializer.Serialize(oldValues ?? new { }),
            NewValuesJson = JsonSerializer.Serialize(newValues ?? new { }),
            TimestampUtc = DateTime.UtcNow
        });
        // Callers are responsible for invoking SaveChangesAsync so that the audit entry
        // and the main entity change are persisted in a single transaction.
        return Task.CompletedTask;
    }
}
