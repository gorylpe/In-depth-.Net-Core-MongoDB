namespace Mongo.Reviews
{
	public class GradeReview : IReview
	{
		public Grade Grade { get; set; }

		public GradeReview()
		{
		}

		public GradeReview(Grade grade)
		{
			Grade = grade;
		}

		public string Print() => $"Only grade: {Grade}";
	}
}