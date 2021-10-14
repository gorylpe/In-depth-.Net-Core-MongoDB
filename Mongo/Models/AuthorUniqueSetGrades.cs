using System.Collections.Generic;
using Mongo.Reviews;

namespace Mongo.Models
{
	public class AuthorUniqueSetGrades
	{
		public string      Author { get; set; }
		public IEnumerable<Grade> Grades { get; set; }

		public override string ToString() => $"{Author} - {string.Join(", ", Grades)}";
	}
}