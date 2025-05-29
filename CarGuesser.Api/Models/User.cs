namespace CarGuesser.Api.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string? Email { get; set; }
        public string PasswordHash { get; set; } = null!;
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

        public List<GameSession> GameSessions { get; set; } = new();
        public List<Car> AddedCars { get; set; } = new();
        public List<Question> AddedQuestions { get; set; } = new();
    }
}
