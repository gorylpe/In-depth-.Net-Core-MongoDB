using System.Collections.Generic;
using Mongo.Reviews;
using MongoDB.Bson;

namespace Mongo.Models
{
	public class BookAuthorFilteredReviewModel
	{
		public string             Author  { get; set; }
		public List<ExpertReview> Reviews { get; set; }
	}
}