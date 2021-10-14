using System;

namespace Mongo
{
	public class BookModel
	{
		public const string DefaultAuthor = "Unknown";

		public string   Idek        { get; set; }
		public string   Title       { get; set; }
		public string   Author      { get; set; } = DefaultAuthor;
		public DateTime ReleaseDate { get; set; }
		public BookType Type        { get; set; }

		public override string ToString() => $"{Idek} - {Title} - {Author} - {ReleaseDate.Year} - {Type}";
	}
}