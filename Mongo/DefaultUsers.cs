using System.Collections.Generic;
using Mongo.Models;

namespace Mongo
{
	public class DefaultUsers
	{
		public static readonly List<UserModel> Users = new()
		{
			new(
				"615b6cacc75a9a794ce2dbfb",
				"Piotr Przestrzelski"
			),
			new(
				"615b6cacc75a9a794ce2dbfc",
				"Jan Kowalski"
			)
		};
	}
}