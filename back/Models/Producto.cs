namespace VentifyAPI.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal PrecioCompra { get; set; }
        public int StockActual { get; set; }
        public int StockMinimo { get; set; } = 1;
        public int CantidadInicial { get; set; }
        public int NegocioId { get; set; }
        public int UsuarioId { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
        public bool Activo { get; set; } = true;

        public Usuario? Usuario { get; set; }
        public Negocio? Negocio { get; set; }
        public ICollection<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();
    }
}
