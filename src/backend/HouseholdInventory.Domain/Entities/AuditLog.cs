using HouseholdInventory.Domain.Common;
using HouseholdInventory.Domain.Enums;

namespace HouseholdInventory.Domain.Entities;

public class AuditLog : BaseEntity
{
    public AuditActionType Action { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public Guid? EntityId { get; set; }
    public string Category { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public Guid? UserId { get; set; }
    public string OldValuesJson { get; set; } = "{}";
    public string NewValuesJson { get; set; } = "{}";
    public string Summary { get; set; } = string.Empty;
    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
}
