using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mongo.Models;
using MongoDB.Bson;

namespace Mongo
{
	public interface IUserRepository
	{
		Task<bool>            AddUsersAsync(List<UserModel> userModels);
		Task<List<UserModel>> GetUsersAsync();
		Task                  RemoveAllUsers();
		Task<bool>            ReserveBookAsync(ObjectId userId, ObjectId bookId, int maxBooksPerUser);
	}
}