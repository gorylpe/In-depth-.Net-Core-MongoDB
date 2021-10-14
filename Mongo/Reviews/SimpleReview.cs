namespace Mongo.Reviews
{
	public class SimpleReview : IReview
	{
		public int Overall { get; set; }

		public string Print()
		{
			return $"People say: {Overall}";
		}
	}
}