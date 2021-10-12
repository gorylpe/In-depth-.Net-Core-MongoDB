using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;

namespace Mongo
{
	public class UserInterfaceService : BackgroundService
	{
		private readonly IHostApplicationLifetime _hostApplicationLifetime;
		private readonly IBookRepository          _bookRepository;

		public UserInterfaceService(IHostApplicationLifetime hostApplicationLifetime, IBookRepository bookRepository)
		{
			_hostApplicationLifetime = hostApplicationLifetime;
			_bookRepository = bookRepository;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			await Task.Delay(1000);
			while (!stoppingToken.IsCancellationRequested)
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
					case "get":
						await GetBooks();
						break;
					case "remove":
						await RemoveBook();
						break;
					case "exit":
						_hostApplicationLifetime.StopApplication();
						return;
					default:
						continue;
				}
			}
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
			var book = new BookModel
			{
				Title = title,
				Author = author,
				ReleaseDate = new DateTime(int.Parse(year!), 1, 1)
			};
			var added = await _bookRepository.AddBook(book);
			Console.WriteLine($"Book{(added ? "" : " not")} added");
		}
	}
}