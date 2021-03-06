using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mongo.Models;
using Mongo.Reviews;
using MongoDB.Bson;

namespace Mongo
{
	public class UserInterfaceService : BackgroundService
	{
		private readonly ILogger<UserInterfaceService> _logger;
		private readonly IHostApplicationLifetime      _hostApplicationLifetime;
		private readonly IBookRepository               _bookRepository;
		private readonly IUserRepository               _userRepository;
		private readonly BookReservationService        _bookReservationService;

		public UserInterfaceService(ILogger<UserInterfaceService> logger, IHostApplicationLifetime hostApplicationLifetime, IBookRepository bookRepository,
			IUserRepository userRepository, BookReservationService bookReservationService)
		{
			_logger = logger;
			_hostApplicationLifetime = hostApplicationLifetime;
			_bookRepository = bookRepository;
			_userRepository = userRepository;
			_bookReservationService = bookReservationService;
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
						#region Old1

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
						case "addrevs":
							await AddSimpleReviewToBook();
							break;
						case "addreve":
							await AddExpertReviewToBook();
							break;
						case "getsimple":
							await GetBooksWithSimpleReviews();
							break;
						case "addrevg":
							await AddGradeReviewToBook();
							break;
						case "getgrade":
							await GetBooksWithGradeReviewsGreaterThan();
							break;

						case "reset":
							await ResetToDefault();
							break;
						case "count":
							await CountBooks();
							break;
						case "countnewer":
							await CountBooksWithReleaseDateGreaterThan();
							break;
						case "countreview":
							await CountBooksWithAtLeastOneReview();
							break;
						case "grouptypes":
							await GroupByTypes();
							break;
						case "groupauthor1":
							await GroupByAuthorsWithAtLeast1Book();
							break;
						case "titles":
							await GetTitles();
							break;
						case "titlesreviews":
							await GetTitlesAndReviewsCount();
							break;
						case "facetbucket":
							await GetBooksCountInCenturiesAndDecades();
							break;
						case "averageexpert":
							await AverageOverallOfExpertReviewsByAuthor();
							break;
						case "uniquegrade":
							await UniqueSetOfGradesByAuthor();
							break;
						case "update50":
							await UpdateBooksRemoveSimpleReviewsWithOverallLessThan50();
							break;

						#endregion

						case "getu":
							await GetUsers();
							break;
						case "reserve":
							await ReserveBook();
							break;
						case "reserves":
							await ReserveBooks();
							break;
						case "reservetest":
							await ReserveBooksOnlyAvailableTest();
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
					_logger.LogError("{Exception}", e.ToString());
				}
			}
		}

		private async Task ReserveBooksOnlyAvailableTest()
		{
			var books = await _bookRepository.GetBooksAsync();
			var users = await _userRepository.GetUsersAsync();
			var t1 = _bookReservationService.ReserveMultipleBooksOnlyAvailable(
				new ObjectId(users[0].Id), 
				books.Skip(1).Take(2).Select(x => new ObjectId(x.Idek)).ToList());
			var t2 = _bookReservationService.ReserveMultipleBooksOnlyAvailable(
				new ObjectId(users[1].Id), 
				books.Take(2).Select(x => new ObjectId(x.Idek)).ToList());
			await Task.WhenAll(t1, t2);
			_logger.LogInformation("{Info}", $"Reserved {t1.Result?.Count ?? -1}, {t2.Result?.Count ?? -1}");
		}

		private async Task ReserveBooks()
		{
			Console.Write("User id: ");
			var userId = ObjectId.Parse(Console.ReadLine());
			Console.Write("Books ids: ");
			var booksIdsStr = Console.ReadLine();
			var booksIds = booksIdsStr!.Split(' ').Select(ObjectId.Parse).ToList();
			var result = await _bookReservationService.ReserveMultipleBooks(userId, booksIds);
			Console.WriteLine($"Books{(result ? "" : " not")} reserved");
		}

		private async Task ReserveBook()
		{
			Console.Write("User id: ");
			var userId = ObjectId.Parse(Console.ReadLine());
			Console.Write("Book id: ");
			var bookId = ObjectId.Parse(Console.ReadLine());
			var result = await _bookReservationService.ReserveSingleBook(userId, bookId);
			Console.WriteLine($"Book{(result ? "" : " not")} reserved");
		}

		private async Task GetUsers()
		{
			var users = await _userRepository.GetUsersAsync();
			var usersStr = string.Join(Environment.NewLine, users);
			Console.WriteLine(usersStr);
		}

		#region Old1

		private async Task UpdateBooksRemoveSimpleReviewsWithOverallLessThan50()
		{
			await _bookRepository.UpdateBooksRemoveSimpleReviewsWithOverallLessThan50Async();
			Console.WriteLine("Books updated");
		}

		private async Task UniqueSetOfGradesByAuthor()
		{
			var grades = await _bookRepository.UniqueSetOfGradesByAuthorAsync();
			var gradesStr = string.Join(Environment.NewLine, grades.Select(x => $"\t{x}"));
			Console.WriteLine(gradesStr);
		}

		private async Task AverageOverallOfExpertReviewsByAuthor()
		{
			var averages = await _bookRepository.AverageOverallOfExpertReviewsByAuthorAsync();
			var averagesStr = string.Join(Environment.NewLine, averages.Select(x => $"\t{x}"));
			Console.WriteLine(averagesStr);
		}

		private async Task GetBooksCountInCenturiesAndDecades()
		{
			var (centuries, decades) = await _bookRepository.GetBooksCountInCenturiesAndDecadesAsync();
			Console.WriteLine("Centuries:");
			var centuriesStr = string.Join(Environment.NewLine, centuries.Select(x => $"\t{x}"));
			Console.WriteLine(centuriesStr);
			Console.WriteLine("Decades:");
			var decadesStr = string.Join(Environment.NewLine, decades.Select(x => $"\t{x}"));
			Console.WriteLine(decadesStr);
		}

		private async Task GetTitlesAndReviewsCount()
		{
			var titlesAndReviewsCount = await _bookRepository.GetBooksTitleAndReviewsCountAsync();
			var titlesAndReviewsCountStr = string.Join(Environment.NewLine, titlesAndReviewsCount);
			Console.WriteLine(titlesAndReviewsCountStr);
		}

		private async Task GetTitles()
		{
			var titles = await _bookRepository.GetBooksTitlesAsync();
			var titlesStr = string.Join(Environment.NewLine, titles);
			Console.WriteLine(titlesStr);
		}

		private async Task GroupByAuthorsWithAtLeast1Book()
		{
			var authorBookCount = await _bookRepository.GroupByAuthorsWithAtLeast1BookAsync();
			Console.WriteLine(string.Join("\n", authorBookCount));
		}

		private async Task GroupByTypes()
		{
			var bookTypeCounts = await _bookRepository.GroupByTypesAsync();
			Console.WriteLine(string.Join("\n", bookTypeCounts));
		}

		private async Task CountBooksWithAtLeastOneReview()
		{
			var count = await _bookRepository.CountBooksWithAtLeastOneReviewAsync();
			Console.WriteLine($"Books count {count}");
		}

		private async Task CountBooksWithReleaseDateGreaterThan()
		{
			Console.Write("Year: ");
			var year = int.Parse(Console.ReadLine()!);
			var count = await _bookRepository.CountBooksNewerThanAsync(new DateTime(year, 1, 1));
			Console.WriteLine($"Books count {count}");
		}

		private async Task ResetToDefault()
		{
			await _bookRepository.RemoveAllBooks();
			await _bookRepository.AddBooksAsync(DefaultBooks.Books);
			await _userRepository.RemoveAllUsers();
			await _userRepository.AddUsersAsync(DefaultUsers.Users);
		}

		private async Task CountBooks()
		{
			var count = await _bookRepository.CountBooksAsync();
			Console.WriteLine($"Books count {count}");
		}

		private async Task GetBooksWithGradeReviewsGreaterThan()
		{
			Console.Write("Grade: ");
			var grade = Enum.Parse<Grade>(Console.ReadLine()!);
			var books = await _bookRepository.GetBooksWithGradeReviewsGreaterThanAsync(grade);
			var booksStr = string.Join(Environment.NewLine, books);
			Console.WriteLine(booksStr);
		}

		private async Task AddGradeReviewToBook()
		{
			Console.Write("Id: ");
			var id = Console.ReadLine();
			var objectId = ObjectId.Parse(id);
			Console.Write("Grade: ");
			var grade = Enum.Parse<Grade>(Console.ReadLine()!);

			var review = new GradeReview
			{
				Grade = grade
			};

			var added = await _bookRepository.AddReviewToBook(objectId, review);
			Console.WriteLine($"Review{(added ? "" : " not")} added");
		}

		private async Task GetBooksWithSimpleReviews()
		{
			var books = await _bookRepository.GetBooksWithSimpleReviewsAsync();
			var booksStr = string.Join(Environment.NewLine, books);
			Console.WriteLine(booksStr);
		}

		private async Task AddSimpleReviewToBook()
		{
			Console.Write("Id: ");
			var id = Console.ReadLine();
			var objectId = ObjectId.Parse(id);
			Console.Write("Overall: ");
			var overall = int.Parse(Console.ReadLine()!);

			var review = new SimpleReview
			{
				Overall = overall
			};

			var added = await _bookRepository.AddReviewToBook(objectId, review);
			Console.WriteLine($"Review{(added ? "" : " not")} added");
		}

		private async Task AddExpertReviewToBook()
		{
			Console.Write("Id: ");
			var id = Console.ReadLine();
			var objectId = ObjectId.Parse(id);
			Console.Write("Overall: ");
			var overall = int.Parse(Console.ReadLine()!);
			Console.Write("Additional word: ");
			var additionalWord = Console.ReadLine();

			var review = new ExpertReview
			{
				Overall = overall,
				AdditionalWord = additionalWord
			};

			var added = await _bookRepository.AddReviewToBook(objectId, review);
			Console.WriteLine($"Review{(added ? "" : " not")} added");
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
			for (var i = 0;
				i < num;
				i++)
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

		#endregion
	}
}