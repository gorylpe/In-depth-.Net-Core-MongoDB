using System.Collections.Generic;
using System.Linq;

namespace Mongo.Models
{
	public class UserModel
	{
		public string       Id            { get; set; }
		public string       Name          { get; set; }
		public List<string> ReservedBooks { get; set; } = new();

		public UserModel(string name)
		{
			Name = name;
		}

		public UserModel(string id, string name) : this(name)
		{
			Id = id;
		}

		public override string ToString() => $"{Id} - {Name} - [{string.Join(", ", ReservedBooks.Select(x => x.ToString()))}]";
	}
}