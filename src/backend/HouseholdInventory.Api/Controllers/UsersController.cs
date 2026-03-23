using HouseholdInventory.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HouseholdInventory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "AdminOnly")]
public class UsersController(UserManager<ApplicationUser> userManager) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> GetUsers()
    {
        var users = await userManager.Users.ToListAsync();
        var response = new List<object>();
        foreach (var user in users)
        {
            response.Add(new { user.Id, user.Email, user.FullName, Roles = await userManager.GetRolesAsync(user), user.IsActive });
        }
        return Ok(response);
    }
}
