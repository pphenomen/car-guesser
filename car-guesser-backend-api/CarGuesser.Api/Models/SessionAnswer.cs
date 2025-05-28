namespace CarGuesser.Api.Models
{
    public class SessionAnswer
    {
        public int Id { get; set; }
        public int GameSessionId { get; set; }
        public GameSession? GameSession { get; set; }
        public int QuestionNumber { get; set; }
        public bool Answer { get; set; }
    }

}
