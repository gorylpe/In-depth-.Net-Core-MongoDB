namespace Mongo.Models
{
	public class BookTitleWithReviewsCount
	{
		public string Title        { get; set; }
		public int    ReviewsCount { get; set; }

		public override string ToString() => $"{Title} - reviews {ReviewsCount}";
	}
}