namespace Mongo.Reviews
{
	public class GradeReview : IReview
	{
		public Grade Grade { get; set; }

		public string Print() => $"Only grade: {Grade}";
	}
}