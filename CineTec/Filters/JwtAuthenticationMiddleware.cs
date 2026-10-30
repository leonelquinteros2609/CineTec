using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using CineTec.Core.Helpers;

namespace CineTec.Api.Filters
{
    public class JwtAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public JwtAuthenticationMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrWhiteSpace(authHeader) && authHeader.StartsWith("Bearer ", System.StringComparison.OrdinalIgnoreCase))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                var jwtKey = _configuration["Jwt:Key"] ?? "CineTec_Super_Secret_Key_For_JWT_Authentication_2026_UCB!";

                var (valid, claims) = JwtHelper.ValidateToken(token, jwtKey);
                if (valid && claims != null)
                {
                    var claimList = new List<Claim>();
                    if (claims.TryGetValue("id", out var id))
                    {
                        claimList.Add(new Claim(ClaimTypes.NameIdentifier, id));
                        claimList.Add(new Claim("id", id));
                    }
                    else if (claims.TryGetValue("sub", out var sub))
                    {
                        claimList.Add(new Claim(ClaimTypes.NameIdentifier, sub));
                        claimList.Add(new Claim("id", sub));
                    }

                    if (claims.TryGetValue("name", out var name))
                    {
                        claimList.Add(new Claim(ClaimTypes.Name, name));
                    }

                    if (claims.TryGetValue("email", out var email))
                    {
                        claimList.Add(new Claim(ClaimTypes.Email, email));
                    }

                    if (claims.TryGetValue("rol", out var rol))
                    {
                        claimList.Add(new Claim(ClaimTypes.Role, rol));
                        claimList.Add(new Claim("rol", rol));
                    }
                    else if (claims.TryGetValue("role", out var role))
                    {
                        claimList.Add(new Claim(ClaimTypes.Role, role));
                        claimList.Add(new Claim("rol", role));
                    }

                    var identity = new ClaimsIdentity(claimList, "Bearer");
                    context.User = new ClaimsPrincipal(identity);
                }
            }

            await _next(context);
        }
    }
}
