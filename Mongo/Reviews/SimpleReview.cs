namespace Mongo.Reviews
{
	public class SimpleReview : IReview
	{
		public int Overall { get; set; }

		public SimpleReview()
		{
		}

		public SimpleReview(int overall)
		{
			Overall = overall;
		}

		public string Print() => $"People say: {Overall}";
	}
}