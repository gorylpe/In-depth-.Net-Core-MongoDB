using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace Mongo
{
	public class ReviewDiscriminatorConvention : ConventionBase, IClassMapConvention
	{
		private const string SuffixToRemove = "Review";

		public void Apply(BsonClassMap classMap)
		{
			var type = classMap.ClassType;
			var discriminator = type.Name;
			if (discriminator.EndsWith(SuffixToRemove))
				discriminator = discriminator.Substring(0, discriminator.Length - SuffixToRemove.Length);
			classMap.SetDiscriminator(discriminator);
		}
	}
}