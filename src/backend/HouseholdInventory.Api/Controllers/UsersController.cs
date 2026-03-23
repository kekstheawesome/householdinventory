using HouseholdInventory.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HouseholdInventory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "AdminOnly")]
public class UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> GetUsers()
    {
        var users = await userManager.Users.ToListAsync();

        // Batch role lookups to avoid N+1 queries.
        var roles = await roleManager.Roles.ToListAsync();
        var userRoles = new Dictionary<string, List<string>>();
        foreach (var role in roles)
        {
            var usersInRole = await userManager.GetUsersInRoleAsync(role.Name!);
            foreach (var userInRole in usersInRole)
            {
                if (!userRoles.TryGetValue(userInRole.Id.ToString(), out var roleList))
                {
                    roleList = new List<string>();
                    userRoles[userInRole.Id.ToString()] = roleList;
                }
                roleList.Add(role.Name!);
            }
        }

        var response = users.Select(user =>
        {
            userRoles.TryGetValue(user.Id.ToString(), out var rolesForUser);
            return new
            {
                user.Id,
                user.Email,
                user.FullName,
                Roles = rolesForUser ?? new List<string>(),
                user.IsActive
            };
        });

        return Ok(response);
    }
}
