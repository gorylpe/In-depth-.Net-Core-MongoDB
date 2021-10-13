using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Mongo
{
	public class AuthorStringSerializer : StringSerializer
	{
		protected override void SerializeValue(BsonSerializationContext context, BsonSerializationArgs args, string value)
		{
			var parts = value.Split(' ');
			for (var i = 0; i < parts.Length; i++)
			{
				var part = parts[i];
				if (part.Length == 0)
						continue;

				var newPart = char.ToUpperInvariant(part[0]).ToString();
				if (part.Length > 1)
					newPart += part.Substring(1).ToLowerInvariant();

				parts[i] = newPart;
			}

			var newValue = string.Join(' ', parts);
			base.SerializeValue(context, args, newValue);
		}
	}
}