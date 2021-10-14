using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mongo.Reviews;
using MongoDB.Bson;

namespace Mongo
{
	public interface IBookRepository
	{
		Task<bool>            AddBookAsync(BookModel bookModel);
		Task<bool>            AddBooksAsync(List<BookModel> bookModels);
		Task<List<BookModel>> GetBooksAsync();
		Task<List<BookModel>> GetBooksByAuthorAsync(string author);
		Task<List<BookModel>> GetBooksNewerThanAsync(DateTime date);
		Task<bool>            RemoveBookAsync(ObjectId id);
		Task<bool>            RemoveBooksAsync(List<ObjectId> ids);
		Task<bool>            AddReviewToBook(ObjectId id, IReview review);
	}
}