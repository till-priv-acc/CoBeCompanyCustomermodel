using CompanyProduct.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace CompanyProduct.Api.Owners;

public static class OwnerEndpoints
{
    public static void MapOwnerEndpoints(this WebApplication app)
    {
        // Gibt alle vorhandenen Owner/Admins zurück
        app.MapGet("/api/admin/owners", async (AppDbContext db) =>
        {
            var owners = await db.Owners
                .Select(owner => new
                {
                    owner.Id,
                    owner.Username
                })
                .ToListAsync();

            return Results.Ok(owners);
        })
        .RequireAuthorization();
    }
}