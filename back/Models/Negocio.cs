namespace VentifyAPI.Models
{
    public class Negocio
    {
        public int Id { get; set; }
        public required string NombreNegocio { get; set; }
        public int OwnerId { get; set; }
        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
