using System;
using System.Threading.Tasks;
using Mongo.Models;
using MongoDB.Bson;

namespace Mongo
{
	public class BookReservationService
	{
		private readonly BookReservationServiceConfig _config;
		private readonly IBookRepository              _bookRepository;
		private readonly IUserRepository              _userRepository;

		public BookReservationService(BookReservationServiceConfig config, IBookRepository bookRepository, IUserRepository userRepository)
		{
			_config = config;
			_bookRepository = bookRepository;
			_userRepository = userRepository;
		}

		public async Task<bool> ReserveSingleBook(ObjectId userId, ObjectId bookId)
		{
			var reserved = await _bookRepository.ReserveBookAsync(bookId, userId);
			if (!reserved)
				return false;
			var bookAdded = await _userRepository.ReserveBookAsync(userId, bookId, _config.MaxBooksPerUser);
			if (!bookAdded)
			{
				await _bookRepository.RemoveBookReservationAsync(bookId);
				return false;
			}

			return true;
		}
	}
}