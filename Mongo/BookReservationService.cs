using System;
using System.Threading.Tasks;
using Mongo.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Mongo
{
	public class BookReservationService
	{
		private readonly BookReservationServiceConfig _config;
		private readonly IMongoClient                 _client;
		private readonly IBookRepository              _bookRepository;
		private readonly IUserRepository              _userRepository;

		public BookReservationService(BookReservationServiceConfig config, IMongoClient client, IBookRepository bookRepository, IUserRepository userRepository)
		{
			_config = config;
			_client = client;
			_bookRepository = bookRepository;
			_userRepository = userRepository;
		}

		public async Task<bool> ReserveSingleBook(ObjectId userId, ObjectId bookId)
		{
			using var session = await _client.StartSessionAsync();
			session.StartTransaction();

			try
			{
				var reserved = await _bookRepository.ReserveBookAsync(bookId, userId);
				if (!reserved)
					return false;
				var bookAdded = await _userRepository.ReserveBookAsync(userId, bookId, _config.MaxBooksPerUser);
				if (!bookAdded)
					return false;
			}
			catch (Exception e)
			{
				await session.AbortTransactionAsync();
				return false;
			}

			await session.CommitTransactionAsync();
			return true;
		}
	}
}