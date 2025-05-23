namespace CarGuesser.Api.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string? Email { get; set; }
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    }
}
