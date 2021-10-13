using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Mongo
{
	public class BookModel
	{
		[BsonId]               public ObjectId Idek  { get; set; }
		[BsonElement("title")] public string   Title { get; set; }

		[BsonDefaultValue("Unknown")]
		[BsonIgnoreIfNull]
		[BsonIgnoreIfDefault]
		[BsonElement("author")]
		public string Author { get; set; }

		[BsonElement("releaseDate")] public DateTime ReleaseDate { get; set; }

		public override string ToString() => $"{Idek.ToString()} - {Title} - {Author} - {ReleaseDate.Year}";
	}
}