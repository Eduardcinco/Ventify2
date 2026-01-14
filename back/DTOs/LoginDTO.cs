namespace VentifyAPI.DTOs
{
    public class LoginDTO
    {
        public string? Correo { get; set; }
        public string? Email { get; set; }
        public required string Password { get; set; }
        public string CorreoNormalizado => Correo ?? Email ?? string.Empty;
    }
}
