using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mongo.Models;
using Mongo.Reviews;
using MongoDB.Bson;
using MongoDB.Driver;
using CPE = Mongo.CustomPipelineExtensions;

namespace Mongo
{
	public class BookRepository : IBookRepository
	{
		private const string CollectionName = "books";

		private readonly IMongoCollection<BookModel> _collection;

		public BookRepository(IMongoDatabase database)
		{
			_collection = database.GetCollection<BookModel>(CollectionName);
		}

		public async Task<bool> AddBookAsync(BookModel bookModel)
		{
			try
			{
				await _collection.InsertOneAsync(bookModel);
				return true;
			}
			catch (MongoWriteException)
			{
				return false;
			}
		}

		public async Task<bool> AddBooksAsync(List<BookModel> bookModels)
		{
			try
			{
				await _collection.InsertManyAsync(bookModels);
				return true;
			}
			catch (MongoWriteException)
			{
				return false;
			}
		}

		public async Task<List<BookModel>> GetBooksAsync()
		{
			var filter = Builders<BookModel>.Filter.Empty;
			return await _collection.Find(filter).ToListAsync();
		}

		public async Task<List<BookModel>> GetBooksByAuthorAsync(string author)
		{
			var filter = Builders<BookModel>.Filter.Eq(x => x.Author, author);
			return await _collection.Find(filter).ToListAsync();
		}

		public async Task<List<BookModel>> GetBooksNewerThanAsync(DateTime date)
		{
			var filter = Builders<BookModel>.Filter.Gt(x => x.ReleaseDate, date);
			return await _collection.Find(filter).ToListAsync();
		}

		public async Task<List<BookModel>> GetBooksWithSimpleReviewsAsync()
		{
			var filter = Builders<BookModel>.Filter.ElemMatch(x => x.Reviews,
				Builders<IReview>.Filter.OfType<SimpleReview>());
			return await _collection.Find(filter).ToListAsync();
		}

		public async Task<List<BookModel>> GetBooksWithGradeReviewsGreaterThanAsync(Grade grade)
		{
			var filter = Builders<BookModel>.Filter.ElemMatch(x => x.Reviews,
				Builders<IReview>.Filter.And(
					Builders<IReview>.Filter.OfType<GradeReview>(),
					Builders<IReview>.Filter.Lt(x => ((GradeReview) x).Grade, grade)));
			return await _collection.Find(filter).ToListAsync();
		}

		public async Task<bool> RemoveBookAsync(ObjectId id)
		{
			var filter = Builders<BookModel>.Filter.Eq(x => x.Idek, id.ToString());
			var result = await _collection.DeleteOneAsync(filter);
			return result.DeletedCount > 0;
		}

		public Task<bool> RemoveBooksAsync(List<ObjectId> ids)
		{
			throw new NotImplementedException();
		}

		public async Task<bool> AddReviewToBook(ObjectId id, IReview review)
		{
			var filter = Builders<BookModel>.Filter.Eq(x => x.Idek, id.ToString());
			var update = Builders<BookModel>.Update.Push(x => x.Reviews, review);
			var result = await _collection.FindOneAndUpdateAsync(filter, update);
			return result != default;
		}

		public async Task RemoveAllBooks()
		{
			var filter = Builders<BookModel>.Filter.Empty;
			await _collection.DeleteManyAsync(filter);
		}

		public async Task<long> CountBooksAsync()
		{
			var countResult = await _collection
				.Aggregate()
				.Count()
				.SingleOrDefaultAsync();
			return countResult?.Count ?? 0;
		}

		public async Task<long> CountBooksNewerThanAsync(DateTime dateTime)
		{
			var countResult = await _collection
				.Aggregate()
				.Match(Builders<BookModel>.Filter.Gt(x => x.ReleaseDate, dateTime))
				.Count()
				.SingleOrDefaultAsync();
			return countResult?.Count ?? 0;
		}

		public async Task<long> CountBooksWithAtLeastOneReviewAsync()
		{
			var countResult = await _collection
				.Aggregate()
				.Match(Builders<BookModel>.Filter.SizeGt(x => x.Reviews, 0))
				.Count()
				.SingleOrDefaultAsync();
			return countResult?.Count ?? 0;
		}

		public async Task<List<BookTypeCount>> GroupByTypesAsync()
		{
			var result = await _collection
				.Aggregate()
				.Group(x => x.Type, grouping => new BookTypeCount
				{
					Type = grouping.Key,
					Count = grouping.Count()
				})
				.ToListAsync();
			return result;
		}

		public async Task<List<AuthorBookCountWithNewestDate>> GroupByAuthorsWithAtLeast1BookAsync()
		{
			var result = await _collection
				.Aggregate()
				.Group(x => x.Author, grouping => new AuthorBookCountWithNewestDate
				{
					Author = grouping.Key,
					Count = grouping.Count(),
					NewestDate = grouping.Max(x => x.ReleaseDate)
				})
				.Match(Builders<AuthorBookCountWithNewestDate>.Filter.Gt(x => x.Count, 1))
				.ToListAsync();
			return result;
		}

		public async Task<List<string>> GetBooksTitlesAsync()
		{
			var result = await _collection
				.Aggregate()
				.Project(x => new
				{
					x.Title
				})
				.ToListAsync();
			return result.Select(x => x.Title).ToList();
		}

		public async Task<List<BookTitleWithReviewsCount>> GetBooksTitleAndReviewsCountAsync()
		{
			var filter = Builders<BookModel>.Filter.Empty;
			var result = await _collection
				.Aggregate()
				.Match(filter)
				.Project(x => new BookTitleWithReviewsCount
				{
					Title = x.Title,
					ReviewsCount = x.Reviews.Count
				})
				.ToListAsync();
			return result;
		}

		public async Task<(List<BookCountByDateStart> Centuries, List<BookCountByDateStart> Decades)> GetBooksCountInCenturiesAndDecadesAsync()
		{
			var centuriesDates = new List<DateTime>();
			for (var i = 1400; i <= 2100; i += 100)
				centuriesDates.Add(new DateTime(i, 1, 1, 0, 0, 0, DateTimeKind.Utc));

			var decadesDates = new List<DateTime>();
			for (var i = 1940; i <= 2030; i += 10)
				decadesDates.Add(new DateTime(i, 1, 1, 0, 0, 0, DateTimeKind.Utc));

			var result = await _collection
				.Aggregate()
				.Facet(AggregateFacet.Create("Centuries",
						new EmptyPipelineDefinition<BookModel>().Bucket(
							x => x.ReleaseDate,
							centuriesDates,
							x => new
							{
								_id = default(DateTime),
								count = x.Count()
							},
							new AggregateBucketOptions<DateTime>
							{
								DefaultBucket = new DateTime(1, 1, 1)
							}
						)
					),
					AggregateFacet.Create("Decades",
						new EmptyPipelineDefinition<BookModel>().Bucket(
							x => x.ReleaseDate,
							decadesDates,
							x => new
							{
								_id = default(DateTime),
								count = x.Count()
							},
							new AggregateBucketOptions<DateTime>
							{
								DefaultBucket = new DateTime(1, 1, 1)
							}
						)
					)
				).SingleAsync();

			var result1 = FacetOutput(new {_id = default(DateTime), count = default(int)}, result.Facets[0]);
			var result2 = FacetOutput(new {_id = default(DateTime), count = default(int)}, result.Facets[1]);

			var centuries = result1.Select(x => new BookCountByDateStart
			{
				DateStart = x._id,
				Count = x.count
			}).ToList();
			var decades = result2.Select(x => new BookCountByDateStart
			{
				DateStart = x._id,
				Count = x.count
			}).ToList();

			return (centuries, decades);
		}

		private static IReadOnlyList<T> FacetOutput<T>(T _, AggregateFacetResult result) => result.Output<T>();

		public async Task<List<AuthorAverageOverallOfExpertReviews>> AverageOverallOfExpertReviewsByAuthorAsync()
		{
			var result = await _collection
				.Aggregate()
				.Project<BookModel, BookAuthorFilteredReviewModel>((s, p) =>
					$@"{{
                    '{p.RenderField(x => x.Author)}' : '${s.RenderField(x => x.Author)}',
                    '{p.RenderField(x => x.Reviews)}' : {{
                        '$filter' : {{
                            'input' : '${s.RenderField(x => x.Reviews)}',
                            'as' : 'item',
                            'cond' : {{
                                '$eq' : ['$$item.{s.RenderDiscriminatorField(x => x.Reviews)}', '{typeof(ExpertReview).RenderDiscriminator()}']
                            }}
                        }}
                    }}
                }}")
				.Unwind<BookAuthorFilteredReviewModel, BookAuthorUnwindReviewModel>(x => x.Reviews)
				.Group(x => x.Author, grouping => new AuthorAverageOverallOfExpertReviews
				{
					Author = grouping.Key,
					Average = grouping.Average(x => ((ExpertReview) x.Reviews).Overall)
				})
				.ToListAsync();

			return result;
		}

		public async Task<List<AuthorUniqueSetGrades>> UniqueSetOfGradesByAuthorAsync()
		{
			var result = await _collection
				.Aggregate()
				.Project<BookModel, BookAuthorFilteredGradeReviewModel>((s, p) =>
					$@"{{
                    '{p.RenderField(x => x.Author)}' : '${s.RenderField(x => x.Author)}',
                    '{p.RenderField(x => x.Reviews)}' : {{
                        '$filter' : {{
                            'input' : '${s.RenderField(x => x.Reviews)}',
                            'as' : 'item',
                            'cond' : {{
                                '$eq' : ['$$item.{s.RenderDiscriminatorField(x => x.Reviews)}', '{typeof(GradeReview).RenderDiscriminator()}']
                            }}
                        }}
                    }}
                }}")
				.Unwind<BookAuthorFilteredGradeReviewModel, BookAuthorUnwindReviewModel>(x => x.Reviews)
				.Group(x => x.Author, grouping => new AuthorUniqueSetGrades
				{
					Author = grouping.Key,
					Grades = grouping.Select(x => ((GradeReview) x.Reviews).Grade).Distinct()
				})
				.ToListAsync();

			return result;
		}

		public async Task UpdateBooksRemoveSimpleReviewsWithOverallLessThan50Async()
		{
			var filter = Builders<BookModel>.Filter.Empty;
			var update = Builders<BookModel>.Update.Pipeline(new EmptyPipelineDefinition<BookModel>()
				.AppendStage<BookModel, BookModel, BookModel>((i, o) => new JsonPipelineStageDefinition<BookModel, BookModel>(
					$@"{{
                        '$set' : {{
                            '{i.RenderField(x => x.Reviews)}' : {{
                                '$filter' : {{
                                    'input' : '${i.RenderField(x => x.Reviews)}',
                                    'as' : 'item',
                                    'cond' : {{
                                        '$not' : {{ 
                                            '$and' : [
                                                {{ '$eq' : ['$$item.{i.RenderDiscriminatorField(x => x.Reviews)}', '{typeof(SimpleReview).RenderDiscriminator()}'] }},
                                                {{ '$lt' : ['$$item.{CPE.RenderField<SimpleReview>(x => x.Overall)}', 50] }}
                                            ]
                                        }}
                                    }}
                                }}
                            }}
                        }}
                    }}"
				)));
			await _collection.UpdateManyAsync(filter, update);
		}

		public async Task<bool> ReserveBookAsync(ObjectId bookId, ObjectId userId)
		{
			var filter = Builders<BookModel>.Filter.Eq(x => x.Idek, bookId.ToString());
			filter &= Builders<BookModel>.Filter.Eq(x => x.ReservedBy, null);

			var update = Builders<BookModel>.Update.Set(x => x.ReservedBy, userId.ToString());
			var result = await _collection.UpdateOneAsync(filter, update);

			return result.ModifiedCount == 1;
		}

		public async Task RemoveBookReservationAsync(ObjectId bookId)
		{
			var filter = Builders<BookModel>.Filter.Eq(x => x.Idek, bookId.ToString());

			var update = Builders<BookModel>.Update.Set(x => x.ReservedBy, null);
			await _collection.UpdateOneAsync(filter, update);
		}
	}
}