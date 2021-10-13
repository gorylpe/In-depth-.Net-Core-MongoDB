using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
	}
}