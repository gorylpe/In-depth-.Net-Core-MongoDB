namespace Mongo.Models
{
	public class AuthorAverageOverallOfExpertReviews
	{
		public string Author  { get; set; }
		public double Average { get; set; }

		public override string ToString() => $"{Author} - {Average}";
	}
}