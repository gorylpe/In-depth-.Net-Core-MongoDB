using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mongo.Models;
using Mongo.Reviews;
using MongoDB.Bson;

namespace Mongo
{
	public interface IBookRepository
	{
		Task<bool>                                                                       AddBookAsync(BookModel bookModel);
		Task<bool>                                                                       AddBooksAsync(List<BookModel> bookModels);
		Task<List<BookModel>>                                                            GetBooksAsync();
		Task<List<BookModel>>                                                            GetBooksByAuthorAsync(string author);
		Task<List<BookModel>>                                                            GetBooksNewerThanAsync(DateTime date);
		Task<List<BookModel>>                                                            GetBooksWithSimpleReviewsAsync();
		Task<List<BookModel>>                                                            GetBooksWithGradeReviewsGreaterThanAsync(Grade grade);
		Task<bool>                                                                       RemoveBookAsync(ObjectId id);
		Task<bool>                                                                       RemoveBooksAsync(List<ObjectId> ids);
		Task<bool>                                                                       AddReviewToBook(ObjectId id, IReview review);
		Task                                                                             RemoveAllBooks();
		Task<long>                                                                       CountBooksAsync();
		Task<long>                                                                       CountBooksNewerThanAsync(DateTime dateTime);
		Task<long>                                                                       CountBooksWithAtLeastOneReviewAsync();
		Task<List<BookTypeCount>>                                                        GroupByTypesAsync();
		Task<List<AuthorBookCountWithNewestDate>>                                        GroupByAuthorsWithAtLeast1BookAsync();
		Task<List<string>>                                                               GetBooksTitlesAsync();
		Task<List<BookTitleWithReviewsCount>>                                            GetBooksTitleAndReviewsCountAsync();
		Task<(List<BookCountByDateStart> Centuries, List<BookCountByDateStart> Decades)> GetBooksCountInCenturiesAndDecadesAsync();
		Task<List<AuthorAverageOverallOfExpertReviews>>                                  AverageOverallOfExpertReviewsByAuthorAsync();
		Task<List<AuthorUniqueSetGrades>>                                                UniqueSetOfGradesByAuthorAsync();
	}
}