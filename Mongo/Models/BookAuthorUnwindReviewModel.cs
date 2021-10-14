using Mongo.Reviews;

namespace Mongo.Models
{
	public class BookAuthorUnwindReviewModel
	{
		public string  Author  { get; set; }
		public IReview Reviews { get; set; }
	}
}