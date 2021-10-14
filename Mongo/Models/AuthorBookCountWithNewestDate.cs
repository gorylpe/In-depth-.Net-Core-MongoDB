using System;

namespace Mongo.Models
{
	public class AuthorBookCountWithNewestDate
	{
		public string   Author     { get; set; }
		public int      Count      { get; set; }
		public DateTime NewestDate { get; set; }

		public override string ToString() => $"{Author} - {Count} - {NewestDate}";
	}
}