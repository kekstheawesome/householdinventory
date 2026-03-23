using HouseholdInventory.Application.DTOs;
using HouseholdInventory.Domain.Entities;
using HouseholdInventory.Domain.Enums;

namespace HouseholdInventory.Application.Interfaces;

public interface ITokenService
{
    AuthResponse CreateToken(ApplicationUser user, IList<string> roles);
}

public interface IAuditService
{
    Task WriteAsync(AuditActionType action, string entityName, Guid? entityId, string category, string summary, object? oldValues, object? newValues, CancellationToken cancellationToken = default);
}

public interface IChatService
{
    Task<ChatResponse> AskAsync(string question, CancellationToken cancellationToken = default);
}

public interface ICurrentUserService
{
    Guid? UserId { get; }
    string Email { get; }
}
