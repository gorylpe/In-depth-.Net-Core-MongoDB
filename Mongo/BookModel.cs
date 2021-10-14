using System;
using System.Collections.Generic;
using System.Text;
using Mongo.Reviews;

namespace Mongo
{
	public class BookModel
	{
		public const string DefaultAuthor = "Unknown";

		public string        Idek        { get; set; }
		public string        Title       { get; set; }
		public string        Author      { get; set; } = DefaultAuthor;
		public DateTime      ReleaseDate { get; set; }
		public BookType      Type        { get; set; }
		public List<IReview> Reviews     { get; set; } = new();

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.AppendLine($"{Idek} - {Title} - {Author} - {ReleaseDate.Year} - {Type}");
			foreach (var review in Reviews) 
				sb.AppendLine(review.Print());

			return sb.ToString();
		}
	}
}