namespace CarGuesser.Api.Models
{
    public class Question
    {
        public int Id { get; set; }

        public string Key { get; set; } = null!;   // ����������� ����
        public string Text { get; set; } = null!;  // ����� �������

        public int? AddedByUserId { get; set; }
        public User? AddedByUser { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<CarQuestionAnswer> CarAnswers { get; set; } = new();
    }
}