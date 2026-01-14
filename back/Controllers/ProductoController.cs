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
    [Route("api/productos")]
    [Authorize]
    public class ProductoController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ITenantContext _tenant;

        public ProductoController(AppDbContext db, ITenantContext tenant)
        {
            _db = db;
            _tenant = tenant;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            if (!_tenant.NegocioId.HasValue) return Unauthorized();
            var productos = await _db.Productos
                .Where(p => p.NegocioId == _tenant.NegocioId.Value)
                .OrderByDescending(p => p.Id)
                .Select(p => new { p.Id, p.Nombre, p.PrecioVenta, p.StockActual, p.StockMinimo })
                .ToListAsync();
            return Ok(productos);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductoCreateDTO dto)
        {
            if (!_tenant.NegocioId.HasValue || !_tenant.UserId.HasValue) return Unauthorized();
            if (string.IsNullOrWhiteSpace(dto.Nombre)) return BadRequest(new { message = "Nombre requerido" });
            if (dto.PrecioVenta <= 0 || dto.CantidadInicial <= 0) return BadRequest(new { message = "Precio y cantidad deben ser mayores a 0" });

            var prod = new Producto
            {
                Nombre = dto.Nombre.Trim(),
                Descripcion = dto.Descripcion,
                PrecioVenta = dto.PrecioVenta,
                PrecioCompra = dto.PrecioCompra,
                CantidadInicial = dto.CantidadInicial,
                StockActual = dto.CantidadInicial,
                StockMinimo = dto.StockMinimo,
                NegocioId = _tenant.NegocioId.Value,
                UsuarioId = _tenant.UserId.Value
            };
            _db.Productos.Add(prod);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Producto creado", prod.Id, prod.Nombre });
        }
    }
}
