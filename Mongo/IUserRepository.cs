using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mongo.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Mongo
{
	public interface IUserRepository
	{
		Task<bool>            AddUsersAsync(List<UserModel> userModels);
		Task<List<UserModel>> GetUsersAsync();
		Task                  RemoveAllUsers();
		Task<bool>            ReserveBookAsync(ObjectId userId, ObjectId bookId, int maxBooksPerUser, IClientSessionHandle handle = null);
		Task<bool>            ReserveBooksAsync(ObjectId userId, List<ObjectId> booksIds, int maxBooksPerUser, IClientSessionHandle handle);
	}
}