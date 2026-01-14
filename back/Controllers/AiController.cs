using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VentifyAPI.Data;
using VentifyAPI.Services;

namespace VentifyAPI.Controllers
{
    [ApiController]
    [Route("api/ai")]
    [Authorize]
    public class AiController : ControllerBase
    {
        private readonly AiService _ai;
        private readonly AppDbContext _db;

        public AiController(AiService ai, AppDbContext db)
        {
            _ai = ai;
            _db = db;
        }

        public class ChatRequest
        {
            public string? model { get; set; }
            public List<AiMessage> messages { get; set; } = new();
        }

        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] ChatRequest req)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("id")?.Value;
            if (!int.TryParse(userIdClaim, out var userId)) return Unauthorized();

            var negocioId = await _db.Usuarios.Where(u => u.Id == userId).Select(u => u.NegocioId).FirstOrDefaultAsync();
            var systemPrompt = "Eres asistente de Ventify. Ayuda sobre inventario, ventas y caja. Responde breve. Negocio=" + negocioId;

            var messages = new List<AiMessage> { new AiMessage { Role = "system", Content = systemPrompt } };
            if (req.messages != null && req.messages.Count > 0) messages.AddRange(req.messages);

            var referer = Request.Headers["Origin"].FirstOrDefault() ?? "https://ventify.local";
            var content = await _ai.ChatAsync(messages, req.model, referer, "Ventify Assistant");
            return Ok(new { message = content });
        }
    }
}
