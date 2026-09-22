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
        app.MapPost("/api/admin/auth/login", async (
            OwnerLoginRequest request,
            AppDbContext db,
            HttpContext httpContext) =>
        {
            var owner = await db.Owners
                .SingleOrDefaultAsync(x => x.Username == request.Username);

            if (owner is null)
                return Results.Unauthorized();

            if (!BCrypt.Net.BCrypt.Verify(request.Password, owner.PasswordHash))
                return Results.Unauthorized();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, owner.Id.ToString()),
                new Claim(ClaimTypes.Name, owner.Username),
                new Claim(ClaimTypes.Role, "Owner")
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            await httpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            return Results.Ok(new
            {
                owner.Id,
                owner.Username
            });
        });
    }
}