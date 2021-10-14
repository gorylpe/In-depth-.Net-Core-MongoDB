using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace Mongo
{
	public class UserInterfaceService : BackgroundService
	{
		private readonly ILogger<UserInterfaceService> _logger;
		private readonly IHostApplicationLifetime      _hostApplicationLifetime;
		private readonly IBookRepository               _bookRepository;

		public UserInterfaceService(ILogger<UserInterfaceService> logger, IHostApplicationLifetime hostApplicationLifetime, IBookRepository bookRepository)
		{
			_logger = logger;
			_hostApplicationLifetime = hostApplicationLifetime;
			_bookRepository = bookRepository;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			await Task.Delay(1000);
			while (!stoppingToken.IsCancellationRequested)
			{
				try
				{
					await Task.Yield();
					Console.Write("Command: ");
					var command = Console.ReadLine();
					switch (command)
					{
						case "hello":
							Console.WriteLine("Hello World!");
							break;
						case "add":
							await AddBook();
							break;
						case "addu":
							await AddBookWithoutAuthor();
							break;
						case "get":
							await GetBooks();
							break;
						case "remove":
							await RemoveBook();
							break;
						case "adds":
							await AddBooks();
							break;
						case "getba":
							await GetBooksByAuthor();
							break;
						case "getnt":
							await GetBooksNewerThan();
							break;
						case "exit":
							_hostApplicationLifetime.StopApplication();
							return;
						default:
							continue;
					}
				}
				catch (Exception e)
				{
					_logger.LogError(e.Message);
				}
			}
		}

		private async Task AddBookWithoutAuthor()
		{
			Console.Write("Title: ");
			var title = Console.ReadLine();
			Console.Write("Year: ");
			var year = Console.ReadLine();
			var book = new BookModel
			{
				Title = title,
				ReleaseDate = new DateTime(int.Parse(year!), 1, 1)
			};
			var added = await _bookRepository.AddBookAsync(book);
			Console.WriteLine($"Book{(added ? "" : " not")} added");
		}

		private async Task GetBooksNewerThan()
		{
			Console.Write("Year: ");
			var year = int.Parse(Console.ReadLine()!);
			var books = await _bookRepository.GetBooksNewerThanAsync(new DateTime(year, 1, 1));
			Console.WriteLine(string.Join("\n", books));
		}

		private async Task GetBooksByAuthor()
		{
			Console.Write("Author: ");
			var author = Console.ReadLine();
			var books = await _bookRepository.GetBooksByAuthorAsync(author);
			Console.WriteLine(string.Join("\n", books));
		}

		private async Task AddBooks()
		{
			Console.Write("Author: ");
			var author = Console.ReadLine();
			Console.Write("Number of books: ");
			var num = int.Parse(Console.ReadLine()!);
			var books = new List<BookModel>();
			for (var i = 0; i < num; i++)
			{
				Console.WriteLine($"Adding book {i + 1}/{num}");
				Console.Write("Title: ");
				var title = Console.ReadLine();
				Console.Write("Year: ");
				var year = int.Parse(Console.ReadLine()!);
				books.Add(new BookModel
				{
					Author = author,
					Title = title,
					ReleaseDate = new DateTime(year, 1, 1)
				});
			}

			var booksAdded = await _bookRepository.AddBooksAsync(books);
			Console.WriteLine($"Books {(booksAdded ? "" : "not ")} added");
		}

		private async Task RemoveBook()
		{
			Console.Write("Id: ");
			var id = Console.ReadLine();
			var objectId = ObjectId.Parse(id);
			var removed = await _bookRepository.RemoveBookAsync(objectId);
			Console.WriteLine($"Book{(removed ? "" : " not")} removed");
		}

		private async Task GetBooks()
		{
			var books = await _bookRepository.GetBooksAsync();
			var booksStr = string.Join(Environment.NewLine, books);
			Console.WriteLine(booksStr);
		}

		private async Task AddBook()
		{
			Console.Write("Title: ");
			var title = Console.ReadLine();
			Console.Write("Author: ");
			var author = Console.ReadLine();
			Console.Write("Year: ");
			var year = Console.ReadLine();
			Console.Write("Type: ");
			var type = Enum.Parse<BookType>(Console.ReadLine()!);
			var book = new BookModel
			{
				Title = title,
				Author = author,
				ReleaseDate = new DateTime(int.Parse(year!), 1, 1),
				Type = type
			};
			var added = await _bookRepository.AddBookAsync(book);
			Console.WriteLine($"Book{(added ? "" : " not")} added");
		}
	}
}