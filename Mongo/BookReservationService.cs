using System;
using System.Collections.Generic;
using System.Linq;
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
				var reserved = await _bookRepository.ReserveBookAsync(bookId, userId, session);
				if (!reserved)
					return false;
				var bookAdded = await _userRepository.ReserveBookAsync(userId, bookId, _config.MaxBooksPerUser, session);
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

		public async Task<bool> ReserveMultipleBooks(ObjectId userId, List<ObjectId> booksIds)
		{
			using var session = await _client.StartSessionAsync();
			return await session.WithTransactionAsync(async (handle, token) =>
			{
				var bookAdded = await _userRepository.ReserveBooksAsync(userId, booksIds, _config.MaxBooksPerUser, handle);
				if (!bookAdded)
				{
					await handle.AbortTransactionAsync(token);
					return false;
				}

				var reserved = await _bookRepository.ReserveBooksAsync(booksIds, userId, handle);
				if (!reserved)
				{
					await handle.AbortTransactionAsync(token);
					return false;
				}

				return true;
			}, new TransactionOptions(maxCommitTime: TimeSpan.FromSeconds(5)));
		}

		public async Task<List<BookModel>> ReserveMultipleBooksOnlyAvailable(ObjectId userId, List<ObjectId> booksIds)
		{
			using var session = await _client.StartSessionAsync();
			return await session.WithTransactionAsync(async (handle, token) =>
			{
				var notReservedBooks = await _bookRepository.GetNotReservedBooksFromList(booksIds, handle);
				var notReservedBooksIds = notReservedBooks.Select(x => new ObjectId(x.Idek)).ToList();
				Console.WriteLine($"RESERVING {notReservedBooks.Count} BOOKS FOR {userId}");

				var bookAdded = await _userRepository.ReserveBooksAsync(userId, notReservedBooksIds, _config.MaxBooksPerUser, handle);
				if (!bookAdded)
				{
					await handle.AbortTransactionAsync(token);
					return null;
				}

				await Task.Delay(500);
				var reserved = await _bookRepository.ReserveBooksAsync(notReservedBooksIds, userId, handle);
				if (!reserved)
				{
					await handle.AbortTransactionAsync(token);
					return null;
				}

				await Task.Delay(500);
				return notReservedBooks;
			}, new TransactionOptions(maxCommitTime: TimeSpan.FromSeconds(5)));
		}
	}
}