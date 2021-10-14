using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MongoDB.Bson.Serialization;

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
	}
}