using HouseholdInventory.Domain.Entities;
using HouseholdInventory.Domain.Enums;
using HouseholdInventory.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HouseholdInventory.Infrastructure.Services;

public static class SeedData
{
    public static async Task InitializeAsync(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
    {
        await dbContext.Database.MigrateAsync();

        foreach (var role in new[] { "Admin", "Roommate" })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }

        async Task EnsureUser(string email, string password, string fullName, string role)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user is null)
            {
                user = new ApplicationUser { UserName = email, Email = email, FullName = fullName, EmailConfirmed = true };
                await userManager.CreateAsync(user, password);
                await userManager.AddToRoleAsync(user, role);
            }
        }

        await EnsureUser("admin@household.local", "Admin123!", "House Admin", "Admin");
        await EnsureUser("roommate@household.local", "Roommate123!", "Helpful Roommate", "Roommate");

        if (!await dbContext.InventoryItems.AnyAsync())
        {
            dbContext.InventoryItems.AddRange(
                new InventoryItem { Name = "Dish Soap", Category = InventoryCategory.Kitchen, Quantity = 1, MinimumStockThreshold = 2, Unit = "bottles", Location = "Under sink", Description = "Lemon scented" },
                new InventoryItem { Name = "Toilet Paper", Category = InventoryCategory.Bathroom, Quantity = 8, MinimumStockThreshold = 6, Unit = "rolls", Location = "Hall closet" },
                new InventoryItem { Name = "Dog Food", Category = InventoryCategory.Pet, Quantity = 4, MinimumStockThreshold = 2, Unit = "lbs", Location = "Pantry" },
                new InventoryItem { Name = "Pasta", Category = InventoryCategory.Pantry, Quantity = 0, MinimumStockThreshold = 2, Unit = "boxes", Location = "Pantry" }
            );
            dbContext.ShoppingListItems.Add(new ShoppingListItem { Name = "Milk", Quantity = 1, Unit = "gallon", Notes = "2%" });
            await dbContext.SaveChangesAsync();
        }
    }
}
