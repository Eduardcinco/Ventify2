namespace VentifyAPI.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public int UsuarioId { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool Revoked { get; set; }

        public Usuario? Usuario { get; set; }
    }
}
