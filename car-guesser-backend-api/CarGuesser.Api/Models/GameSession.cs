namespace CarGuesser.Api.Models
{
    public class GameSession
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public string? GuessedCharacter { get; set; }
        public string? AddedCharacter { get; set; }
    }

}
