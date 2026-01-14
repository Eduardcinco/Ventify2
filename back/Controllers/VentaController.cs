using System.Linq;
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
    [Route("api/ventas")]
    [Authorize]
    public class VentaController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ITenantContext _tenant;

        public VentaController(AppDbContext db, ITenantContext tenant)
        {
            _db = db;
            _tenant = tenant;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VentaCreateDTO dto)
        {
            if (!_tenant.NegocioId.HasValue || !_tenant.UserId.HasValue) return Unauthorized();
            if (dto.Items == null || dto.Items.Count == 0) return BadRequest(new { message = "Items requeridos" });

            var negocioId = _tenant.NegocioId.Value;
            decimal totalCalc = 0;

            foreach (var item in dto.Items)
            {
                var prod = await _db.Productos.FirstOrDefaultAsync(p => p.Id == item.ProductoId && p.NegocioId == negocioId);
                if (prod == null) return BadRequest(new { message = $"Producto {item.ProductoId} no encontrado" });
                if (item.Cantidad <= 0) return BadRequest(new { message = "Cantidad inválida" });
                if (prod.StockActual < item.Cantidad) return BadRequest(new { message = $"Stock insuficiente para {prod.Nombre}" });
                totalCalc += item.Precio * item.Cantidad;
            }

            var venta = new Venta
            {
                NegocioId = negocioId,
                UsuarioId = _tenant.UserId.Value,
                TotalPagado = totalCalc,
                FormaPago = string.IsNullOrWhiteSpace(dto.PaymentMethod) ? "efectivo" : dto.PaymentMethod.Trim().ToLower(),
                MontoRecibido = dto.MontoRecibido,
                Cambio = dto.Cambio
            };
            _db.Ventas.Add(venta);
            await _db.SaveChangesAsync();

            foreach (var item in dto.Items)
            {
                var prod = await _db.Productos.FirstAsync(p => p.Id == item.ProductoId && p.NegocioId == negocioId);
                prod.StockActual -= item.Cantidad;
                var det = new DetalleVenta
                {
                    VentaId = venta.Id,
                    ProductoId = prod.Id,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = item.Precio,
                    Subtotal = item.Cantidad * item.Precio
                };
                _db.DetallesVenta.Add(det);
            }

            await _db.SaveChangesAsync();
            return Ok(new { message = "Venta registrada", venta.Id, total = totalCalc });
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            if (!_tenant.NegocioId.HasValue) return Unauthorized();
            var ventas = await _db.Ventas
                .Where(v => v.NegocioId == _tenant.NegocioId.Value)
                .OrderByDescending(v => v.FechaHora)
                .Take(50)
                .Select(v => new { v.Id, v.TotalPagado, v.FormaPago, v.FechaHora })
                .ToListAsync();
            return Ok(ventas);
        }
    }
}
