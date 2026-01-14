namespace VentifyAPI.DTOs
{
    public class ProductoCreateDTO
    {
        public required string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal PrecioCompra { get; set; }
        public int CantidadInicial { get; set; }
        public int StockMinimo { get; set; } = 1;
    }
}
