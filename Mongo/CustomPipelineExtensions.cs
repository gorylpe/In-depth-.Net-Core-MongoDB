using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Mongo
{
	public static class CustomPipelineExtensions
	{
		public static string RenderField<T>(string field)
		{
			return BsonClassMap.LookupClassMap(typeof(T)).GetMemberMap(field).ElementName;
		}
		public static string RenderField<T>(Expression<Func<T, object>> field)
		{
			return RenderField<T>(GetMemberName(field));
		}

		public static string GetMemberName<T, TField>(Expression<Func<T, TField>> field)
		{
			var memberExpression = field.Body as MemberExpression ?? ((UnaryExpression) field.Body).Operand as MemberExpression;
			return memberExpression!.Member.Name;
		}

		public static string RenderDiscriminatorField<T, TField>(Expression<Func<T, ICollection<TField>>> field)
		{
			return BsonSerializer.LookupDiscriminatorConvention(typeof(TField)).ElementName;
		}
		
		public static string RenderDiscriminator(this Type type)
		{
			return BsonClassMap.LookupClassMap(type).Discriminator;
		}

		public class Renderer<T>
		{
			public static Renderer<T> Instance => null;
		}

		public static string RenderField<T>(this Renderer<T> _, Expression<Func<T, object>> field) => RenderField(field);

		public static string RenderDiscriminatorField<T, TField>(this Renderer<T> _, Expression<Func<T, ICollection<TField>>> field)
		{
			return BsonSerializer.LookupDiscriminatorConvention(typeof(TField)).ElementName;
		}
		
		public static IAggregateFluent<TProjection> Project<TSource, TProjection>(this IAggregateFluent<TSource> aggregation,
			Func<Renderer<TSource>, Renderer<TProjection>, string> render)
		{
			var source = Renderer<TSource>.Instance;
			var projection = Renderer<TProjection>.Instance;
			return aggregation.Project((ProjectionDefinition<TSource, TProjection>) render.Invoke(source, projection));
		}

		public static PipelineDefinition<TInput, TOutput> AppendStage<TInput, TIntermediate, TOutput>(
			this PipelineDefinition<TInput, TIntermediate> pipeline,
			Func<Renderer<TIntermediate>, Renderer<TOutput>, JsonPipelineStageDefinition<TIntermediate, TOutput>> render)
		{
			var intermediate = Renderer<TIntermediate>.Instance;
			var projection = Renderer<TOutput>.Instance;
			return pipeline.AppendStage(render.Invoke(intermediate, projection));
		}
	}
}