namespace VentifyAPI.Models
{
    public class Venta
    {
        public int Id { get; set; }
        public int NegocioId { get; set; }
        public int UsuarioId { get; set; }
        public decimal TotalPagado { get; set; }
        public string FormaPago { get; set; } = "efectivo";
        public decimal? MontoRecibido { get; set; }
        public decimal? Cambio { get; set; }
        public DateTime FechaHora { get; set; } = DateTime.UtcNow;

        public Usuario? Usuario { get; set; }
        public Negocio? Negocio { get; set; }
        public ICollection<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();
    }
}
