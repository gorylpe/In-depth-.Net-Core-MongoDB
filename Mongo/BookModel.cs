using System;
using MongoDB.Bson;

namespace Mongo
{
	public class BookModel
	{
		public ObjectId Id          { get; set; }
		public string   Title       { get; set; }
		public string   Author      { get; set; }
		public DateTime ReleaseDate { get; set; }

		public override string ToString() => $"{Id.ToString()} - {Title} - {Author} - {ReleaseDate.Year}";
	}
}