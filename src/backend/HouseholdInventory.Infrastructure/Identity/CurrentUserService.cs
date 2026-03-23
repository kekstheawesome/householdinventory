using System.Security.Claims;
using HouseholdInventory.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace HouseholdInventory.Infrastructure.Identity;

public class CurrentUserService(IHttpContextAccessor accessor) : ICurrentUserService
{
    public Guid? UserId => Guid.TryParse(accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;
    public string Email => accessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email) ?? "system@local";
}
