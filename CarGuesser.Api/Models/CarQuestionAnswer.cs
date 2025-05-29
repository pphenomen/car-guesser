namespace CarGuesser.Api.Models
{
    public class CarQuestionAnswer
    {
        public int CarId { get; set; }
        public Car? Car { get; set; }
        public int QuestionId { get; set; }
        public Question? Question { get; set; }
        public bool Answer { get; set; }
    }
}