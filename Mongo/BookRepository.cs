using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mongo.Models;
using Mongo.Reviews;
using MongoDB.Bson;
using MongoDB.Driver;

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
	}
}