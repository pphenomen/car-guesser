namespace CarGuesser.Api.Models
{
    public class GameSession
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? EndedAt { get; set; }
        public bool IsSuccess { get; set; }
        public string? GuessedCar { get; set; }

        // Фича: если машина принадлежит игроку
        public string? OwnerName { get; set; }
        public string? OwnerClub { get; set; }

        public string? AddedCar { get; set; }

        public List<SessionAnswer> Answers { get; set; } = new();
    }

}
