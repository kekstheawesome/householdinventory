using System.Net.Http.Json;
using HouseholdInventory.Application.DTOs;
using HouseholdInventory.Application.Interfaces;
using HouseholdInventory.Infrastructure.Configuration;
using HouseholdInventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace HouseholdInventory.Infrastructure.Services;

public class GeminiChatService(ApplicationDbContext dbContext, IHttpClientFactory httpClientFactory, IOptions<GeminiOptions> options) : IChatService
{
    private readonly GeminiOptions _options = options.Value;

    public async Task<ChatResponse> AskAsync(string question, CancellationToken cancellationToken = default)
    {
        var lowStock = await dbContext.InventoryItems.Where(x => x.Quantity <= x.MinimumStockThreshold).Select(x => new { x.Name, x.Quantity, x.MinimumStockThreshold, x.Category }).ToListAsync(cancellationToken);
        var recentAudit = await dbContext.AuditLogs.OrderByDescending(x => x.TimestampUtc).Take(10).Select(x => new { x.Action, x.Category, x.Summary, x.UserEmail, x.TimestampUtc }).ToListAsync(cancellationToken);
        var shopping = await dbContext.ShoppingListItems.Where(x => !x.IsCompleted).Select(x => new { x.Name, x.Quantity, x.Unit }).ToListAsync(cancellationToken);

        var context = new { question, lowStock, recentAudit, shopping };

        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            return new ChatResponse("Gemini API key is not configured. Returning structured context only.", context);
        }

        var prompt = $"You are an assistant for a shared household inventory. Answer using only this JSON context: {System.Text.Json.JsonSerializer.Serialize(context)}";
        var client = httpClientFactory.CreateClient();
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_options.Model}:generateContent?key={_options.ApiKey}";
        var payload = new
        {
            contents = new[] { new { parts = new[] { new { text = prompt } } } }
        };
        var response = await client.PostAsJsonAsync(url, payload, cancellationToken);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadFromJsonAsync<GeminiResponse>(cancellationToken: cancellationToken);
        var answer = json?.candidates?.FirstOrDefault()?.content?.parts?.FirstOrDefault()?.text ?? "No response from Gemini.";
        return new ChatResponse(answer, context);
    }

    public class GeminiResponse
    {
        public List<Candidate>? candidates { get; set; }
    }
    public class Candidate { public Content? content { get; set; } }
    public class Content { public List<Part>? parts { get; set; } }
    public class Part { public string? text { get; set; } }
}
