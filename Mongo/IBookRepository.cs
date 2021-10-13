using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Mongo
{
	public interface IBookRepository
	{
		Task<bool>            AddBookAsync(BookModel bookModel);
		Task<bool>            AddBooksAsync(List<BookModel> bookModels);
		Task<List<BookModel>> GetBooksAsync();
		Task<List<BookModel>> GetBooksByAuthorAsync(string author);
		Task<bool>            RemoveBookAsync(ObjectId id);
	}
}