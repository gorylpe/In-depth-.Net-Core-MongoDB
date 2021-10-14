﻿using System;
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
			BsonClassMap.RegisterClassMap<BookModel>(cm =>
			{
				cm.MapIdProperty(x => x.Idek)
					.SetSerializer(new StringSerializer(BsonType.ObjectId))
					.SetIdGenerator(StringObjectIdGenerator.Instance);
				cm.MapProperty(x => x.Title)
					.SetElementName("title");
				cm.MapProperty(x => x.Author)
					.SetElementName("author")
					.SetDefaultValue(BookModel.DefaultAuthor)
					.SetIgnoreIfDefault(true)
					.SetSerializer(new AuthorStringSerializer());
				cm.MapProperty(x => x.ReleaseDate)
					.SetElementName("releaseDate")
					.SetSerializer(new DateTimeSerializer(true));
				cm.MapProperty(x => x.Type)
					.SetElementName("type")
					.SetSerializer(new EnumSerializer<BookType>(BsonType.String));
				cm.MapProperty(x => x.Reviews)
					.SetElementName("reviews");

				cm.SetIgnoreExtraElements(true);
			});

			BsonClassMap.RegisterClassMap<SimpleReview>();
			BsonClassMap.RegisterClassMap<ExpertReview>();
			BsonSerializer.RegisterDiscriminatorConvention(typeof(IReview), StandardDiscriminatorConvention.Scalar);
		}
	}
}