using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using VentifyAPI.Models;

namespace VentifyAPI.Services
{
    public interface ITokenService
    {
        (string accessToken, string refreshToken, DateTime refreshExpiresAt) CreateTokens(Usuario user);
    }

    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public (string accessToken, string refreshToken, DateTime refreshExpiresAt) CreateTokens(Usuario user)
        {
            var secret = Environment.GetEnvironmentVariable("JWT_SECRET")
                ?? _config["JWT_SECRET"]
                ?? _config["Jwt:Key"]
                ?? "dev-secret-key-minimo-32-caracteres";

            var expiresMinutes = int.TryParse(
                Environment.GetEnvironmentVariable("JWT_EXPIRES_MINUTES")
                ?? _config["JWT_EXPIRES_MINUTES"]
                ?? _config["Jwt:ExpiresInMinutes"],
                out var m) ? m : 60;

            var refreshDays = int.TryParse(
                Environment.GetEnvironmentVariable("REFRESH_EXPIRES_DAYS")
                ?? _config["REFRESH_EXPIRES_DAYS"]
                ?? _config["Jwt:RefreshExpirationDays"],
                out var d) ? d : 30;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var role = user.Rol ?? "dueno";
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Correo ?? string.Empty),
                new Claim(ClaimTypes.Role, role),
                new Claim("rol", role)
            };

            if (user.NegocioId.HasValue)
            {
                claims.Add(new Claim("negocioId", user.NegocioId.Value.ToString()));
            }

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
                signingCredentials: creds
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            var refreshToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray()) + "." + Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            var refreshExpiresAt = DateTime.UtcNow.AddDays(refreshDays);

            return (accessToken, refreshToken, refreshExpiresAt);
        }
    }
}
