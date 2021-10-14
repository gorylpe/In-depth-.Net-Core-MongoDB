namespace Mongo.Reviews
{
	public class SimpleReview : IReview
	{
		public int Overall { get; }

		public string Print()
		{
			return $"People say: {Overall}";
		}
	}
}