using SQLite;

namespace Tsundoku.Models;

[Table("Books")]
public class Book
{
	[PrimaryKey, AutoIncrement]
	public int Id { get; set; }

	public string Title { get; set; } = string.Empty;
	public string Author { get; set; } = string.Empty;
	public DateTime DateBought { get; set; } = DateTime.Today;
	public string ReasonForBuying { get; set; } = string.Empty;
	public BookStatus Status { get; set; } = BookStatus.Bought;

	public string? ImagePath { get; set; }
	public string? Isbn { get; set; }
	public string? ShortDescription { get; set; }
	public DateTime? DateStartedReading { get; set; }
	public DateTime? DateFinishedReading { get; set; }
	public string? Review { get; set; }
	public int? Rating { get; set; }

	public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}
