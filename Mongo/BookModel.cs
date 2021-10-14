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

		public BookModel()
		{
		}

		public BookModel(string title, string author, DateTime releaseDate, BookType type, List<IReview> reviews)
		{
			Title = title;
			Author = author;
			ReleaseDate = releaseDate;
			Type = type;
			Reviews = reviews;
		}

		public BookModel(string idek, string title, string author, DateTime releaseDate, BookType type, List<IReview> reviews) 
			: this(title, author, releaseDate, type, reviews)
		{
			Idek = idek;
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.AppendLine($"{Idek} - {Title} - {Author} - {ReleaseDate.Year} - {Type}");
			foreach (var review in Reviews)
				sb.AppendLine($"\t{review.Print()}");

			return sb.ToString();
		}
	}
}