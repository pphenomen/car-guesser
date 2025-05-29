namespace CarGuesser.Api.Models
{
    public class Question
    {
        public int Id { get; set; }

        public string Key { get; set; } = null!;   // Технический ключ
        public string Text { get; set; } = null!;  // Текст вопроса

        public int? AddedByUserId { get; set; }
        public User? AddedByUser { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<CarQuestionAnswer> CarAnswers { get; set; } = new();
    }
}