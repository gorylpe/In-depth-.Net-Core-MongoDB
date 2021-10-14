namespace Mongo.Reviews
{
	public class ExpertReview : IReview
	{
		public int    Overall        { get; set; }
		public string AdditionalWord { get; set; }

		public string Print() => $"Expert says: {Overall} {AdditionalWord}";
	}
}