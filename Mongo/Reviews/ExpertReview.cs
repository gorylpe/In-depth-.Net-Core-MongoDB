namespace Mongo.Reviews
{
	public class ExpertReview : IReview
	{
		public int    Overall        { get; set; }
		public string AdditionalWord { get; set; }

		public ExpertReview()
		{
		}

		public ExpertReview(int overall, string additionalWord)
		{
			Overall = overall;
			AdditionalWord = additionalWord;
		}

		public string Print() => $"Expert says: {Overall} {AdditionalWord}";
	}
}