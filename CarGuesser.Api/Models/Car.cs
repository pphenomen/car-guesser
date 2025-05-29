namespace CarGuesser.Api.Models
{
	public class Car
	{
		public int Id { get; set; }

		public string Name { get; set; } = null!;

		public string? OwnerName { get; set; }
		public string? OwnerClub { get; set; }

		public int? AddedByUserId { get; set; }
		public User? AddedByUser { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public List<CarQuestionAnswer> Answers { get; set; } = new();
	}

}

