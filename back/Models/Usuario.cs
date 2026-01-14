namespace VentifyAPI.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }
        public required string Correo { get; set; }
        public required string Password { get; set; }
        public int? NegocioId { get; set; }
        public Negocio? Negocio { get; set; }
        public string Rol { get; set; } = "dueño";
        public int TokenVersion { get; set; } = 0;
        public bool PrimerAcceso { get; set; } = false;
        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
    }
}
