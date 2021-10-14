using System;
using System.Diagnostics;
using System.Linq;
using Mongo.Reviews;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

namespace Mongo
{
	public static class BookModelSetup
	{
		public static void Setup()
		{
			var allConventionPack = new ConventionPack
			{
				new CamelCaseElementNameConvention(),
				new StringIdStoredAsObjectIdConvention()
			};
			ConventionRegistry.Register("All", allConventionPack, _ => true);
			
			var reviewType = typeof(IReview);
			var reviewConventionPack = new ConventionPack
			{
				new ReviewDiscriminatorConvention()
			};
			ConventionRegistry.Register("Review", reviewConventionPack, type =>
			{
				var contains = type.GetInterfaces().Contains(reviewType);
				return contains;
			});
			
			BsonClassMap.RegisterClassMap<BookModel>(cm =>
			{
				cm.MapIdMember(x => x.Idek);
				cm.AutoMap();
				cm.GetMemberMap(x => x.Author)
					.SetDefaultValue(BookModel.DefaultAuthor)
					.SetIgnoreIfDefault(true)
					.SetSerializer(new AuthorStringSerializer());
				cm.GetMemberMap(x => x.ReleaseDate)
					.SetSerializer(new DateTimeSerializer(true));
				cm.GetMemberMap(x => x.Type)
					.SetSerializer(new EnumSerializer<BookType>(BsonType.String));

				cm.SetIgnoreExtraElements(true);
			});

			BsonClassMap.RegisterClassMap<SimpleReview>();
			BsonClassMap.RegisterClassMap<ExpertReview>();
			BsonClassMap.RegisterClassMap<GradeReview>(cm =>
			{
				cm.AutoMap();
				cm.GetMemberMap(x => x.Grade)
					.SetSerializer(new EnumSerializer<Grade>(BsonType.String));
			});
			BsonSerializer.RegisterDiscriminatorConvention(typeof(IReview), StandardDiscriminatorConvention.Scalar);
		}
	}
}