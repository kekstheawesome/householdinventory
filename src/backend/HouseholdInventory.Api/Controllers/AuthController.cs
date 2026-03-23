using HouseholdInventory.Application.DTOs;
using HouseholdInventory.Application.Interfaces;
using HouseholdInventory.Domain.Entities;
using HouseholdInventory.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HouseholdInventory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(UserManager<ApplicationUser> userManager, ITokenService tokenService, IAuditService auditService) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null || !await userManager.CheckPasswordAsync(user, request.Password))
        {
            return Unauthorized();
        }

        var roles = await userManager.GetRolesAsync(user);
        await auditService.WriteAsync(AuditActionType.Login, nameof(ApplicationUser), user.Id, "Authentication", $"{user.Email} logged in.", null, new { user.Email, roles });
        return Ok(tokenService.CreateToken(user, roles));
    }

    [HttpPost("users")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult> CreateUser(RegisterUserRequest request)
    {
        var user = new ApplicationUser { UserName = request.Email, Email = request.Email, FullName = request.FullName, EmailConfirmed = true };
        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }
        await userManager.AddToRoleAsync(user, request.Role);
        await auditService.WriteAsync(AuditActionType.UserManagement, nameof(ApplicationUser), user.Id, request.Role, $"Created user {request.Email}", null, request);
        return CreatedAtAction(nameof(CreateUser), new { user.Id });
    }
}
