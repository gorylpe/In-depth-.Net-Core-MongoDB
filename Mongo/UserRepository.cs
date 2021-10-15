using System.Collections.Generic;
using System.Threading.Tasks;
using Mongo.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Mongo
{
	public class UserRepository : IUserRepository
	{
		private const string CollectionName = "users";

		private readonly IMongoCollection<UserModel> _collection;

		public UserRepository(IMongoDatabase database)
		{
			_collection = database.GetCollection<UserModel>(CollectionName);
		}

		public async Task<bool> AddUsersAsync(List<UserModel> userModels)
		{
			try
			{
				await _collection.InsertManyAsync(userModels);
				return true;
			}
			catch (MongoWriteException)
			{
				return false;
			}
		}

		public async Task<List<UserModel>> GetUsersAsync()
		{
			var filter = Builders<UserModel>.Filter.Empty;
			return await _collection.Find(filter).ToListAsync();
		}

		public async Task RemoveAllUsers()
		{
			var filter = Builders<UserModel>.Filter.Empty;
			await _collection.DeleteManyAsync(filter);
		}

		public async Task<bool> ReserveBookAsync(ObjectId userId, ObjectId bookId, int maxBooksPerUser, IClientSessionHandle handle = null)
		{
			var filter = Builders<UserModel>.Filter.Eq(x => x.Id, userId.ToString());
			filter &= Builders<UserModel>.Filter.SizeLt(x => x.ReservedBooks, maxBooksPerUser);

			var update = Builders<UserModel>.Update.Push(x => x.ReservedBooks, bookId.ToString());

			var result = handle == null
				? await _collection.UpdateOneAsync(filter, update)
				: await _collection.UpdateOneAsync(handle, filter, update);
			return result.ModifiedCount == 1;
		}
	}
}