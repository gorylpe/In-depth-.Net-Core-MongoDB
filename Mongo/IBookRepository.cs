using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Mongo
{
	public interface IBookRepository
	{
		Task<bool>            AddBook(BookModel bookModel);
		Task<List<BookModel>> GetBooksAsync();
		Task<bool>            RemoveBookAsync(ObjectId id);
	}
}