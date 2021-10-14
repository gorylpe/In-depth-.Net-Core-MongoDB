using System.Collections.Generic;
using Mongo.Reviews;
using MongoDB.Bson;

namespace Mongo.Models
{
	public class BookAuthorFilteredGradeReviewModel
	{
		public string            Author  { get; set; }
		public List<GradeReview> Reviews { get; set; }
	}
}