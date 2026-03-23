using HouseholdInventory.Application.DTOs;
using HouseholdInventory.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HouseholdInventory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "HouseholdMember")]
public class ChatController(IChatService chatService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ChatResponse>> Ask(ChatRequest request)
        => Ok(await chatService.AskAsync(request.Question));
}
