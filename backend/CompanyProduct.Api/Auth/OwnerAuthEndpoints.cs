using System.Security.Claims;
using CompanyProduct.Api.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace CompanyProduct.Api.Auth;

public static class OwnerAuthEndpoints
{
    public static void MapOwnerAuthEndpoints(this WebApplication app)
    {
        // Owner Login
        app.MapPost("/api/admin/auth/login", async (
            OwnerLoginRequest request,
            AppDbContext db,
            HttpContext httpContext) =>
        {
            // Owner aus der Datenbank laden
            var owner = await db.Owners
                .SingleOrDefaultAsync(x => x.Username == request.Username);

            if (owner is null)
                return Results.Unauthorized();

            // Passwort mit BCrypt prüfen
            if (!BCrypt.Net.BCrypt.Verify(request.Password, owner.PasswordHash))
                return Results.Unauthorized();

            // Daten für die eingeloggte Session
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, owner.Id.ToString()),
                new Claim(ClaimTypes.Name, owner.Username),
                new Claim(ClaimTypes.Role, "Owner")
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            // Auth-Cookie erstellen
            await httpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            return Results.Ok(new
            {
                owner.Id,
                owner.Username
            });
        });


        // Prüft die aktuelle Owner-Session
        app.MapGet("/api/admin/auth/me", (ClaimsPrincipal user) =>
        {
            var id = user.FindFirstValue(ClaimTypes.NameIdentifier);
            var username = user.FindFirstValue(ClaimTypes.Name);

            return Results.Ok(new
            {
                Id = id,
                Username = username
            });
        })
        .RequireAuthorization();

        // Beendet die aktuelle Owner-Session
        app.MapPost("/api/admin/auth/logout", async (HttpContext httpContext) =>
        {
            // Auth-Cookie entfernen
            await httpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return Results.Ok();
        })
        .RequireAuthorization();
    }
}