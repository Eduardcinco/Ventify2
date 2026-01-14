using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VentifyAPI.Data;
using VentifyAPI.DTOs;
using VentifyAPI.Models;
using VentifyAPI.Services;

namespace VentifyAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ITokenService _tokens;

        public AuthController(AppDbContext db, ITokenService tokens)
        {
            _db = db;
            _tokens = tokens;
        }

        private static CookieOptions BuildCookieOptions(DateTime? expiresAt, bool dev)
        {
            return new CookieOptions
            {
                HttpOnly = true,
                Secure = !dev,
                SameSite = dev ? SameSiteMode.Lax : SameSiteMode.None,
                Path = "/",
                Expires = expiresAt
            };
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegistroUsuarioDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre) || string.IsNullOrWhiteSpace(dto.Correo) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest(new { message = "Todos los campos son obligatorios" });

            var exists = await _db.Usuarios.AnyAsync(u => u.Correo == dto.Correo);
            if (exists) return BadRequest(new { message = "El correo ya existe" });

            var user = new Usuario
            {
                Nombre = dto.Nombre,
                Correo = dto.Correo,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Rol = "dueno"
            };
            _db.Usuarios.Add(user);
            await _db.SaveChangesAsync();

            var negocio = new Negocio
            {
                NombreNegocio = string.IsNullOrWhiteSpace(dto.Nombre) ? "Mi negocio" : $"{dto.Nombre} - Negocio",
                OwnerId = user.Id
            };
            _db.Negocios.Add(negocio);
            await _db.SaveChangesAsync();
            user.NegocioId = negocio.Id;
            await _db.SaveChangesAsync();

            var (access, refresh, expires) = _tokens.CreateTokens(user);
            var rt = new RefreshToken { Token = refresh, UsuarioId = user.Id, ExpiresAt = expires, Revoked = false };
            _db.RefreshTokens.Add(rt);
            await _db.SaveChangesAsync();

            var isDev = string.Equals(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), "Development", StringComparison.OrdinalIgnoreCase);
            Response.Cookies.Append("access_token", access, BuildCookieOptions(expires, isDev));
            Response.Cookies.Append("refresh_token", refresh, BuildCookieOptions(expires.AddDays(7), isDev));

            return Ok(new { message = "Registrado", accessToken = access, refreshToken = refresh, usuario = new { user.Id, user.Nombre, user.Correo, user.Rol, user.NegocioId } });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.CorreoNormalizado) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest(new { message = "Correo y contraseña requeridos" });

            var user = await _db.Usuarios.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Correo == dto.CorreoNormalizado);
            if (user == null) return Unauthorized(new { message = "Credenciales inválidas" });
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password)) return Unauthorized(new { message = "Credenciales inválidas" });

            if (user.NegocioId == null)
            {
                var negocio = new Negocio { NombreNegocio = string.IsNullOrWhiteSpace(user.Nombre) ? "Mi negocio" : $"{user.Nombre} - Negocio", OwnerId = user.Id };
                _db.Negocios.Add(negocio);
                await _db.SaveChangesAsync();
                user.NegocioId = negocio.Id;
                await _db.SaveChangesAsync();
            }

            var (access, refresh, expires) = _tokens.CreateTokens(user);
            var rt = new RefreshToken { Token = refresh, UsuarioId = user.Id, ExpiresAt = expires, Revoked = false };
            _db.RefreshTokens.Add(rt);
            await _db.SaveChangesAsync();

            var isDev = string.Equals(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), "Development", StringComparison.OrdinalIgnoreCase);
            Response.Cookies.Append("access_token", access, BuildCookieOptions(expires, isDev));
            Response.Cookies.Append("refresh_token", refresh, BuildCookieOptions(expires.AddDays(7), isDev));

            return Ok(new { message = "Login ok", accessToken = access, refreshToken = refresh, usuario = new { user.Id, user.Nombre, user.Correo, user.Rol, user.NegocioId } });
        }
    }
}
