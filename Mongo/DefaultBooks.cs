using System;
using System.Collections.Generic;
using Mongo.Reviews;

namespace Mongo
{
	public class DefaultBooks
	{
		public static readonly List<BookModel> Books = new()
		{
			new(
				"6168656cdd6f908005daabf8",
				"O obrotach sfer niebieskich",
				"Mikołaj Kopernik",
				new DateTime(1543, 1, 1),
				BookType.Article,
				new List<IReview>
				{
					new ExpertReview(13, "Herezje"),
					new SimpleReview(78),
					new SimpleReview(56),
					new GradeReview(Grade.E)
				}),
			new(
				"6168656cdd6f908005daabf9",
				"Klasyczna teoria pola",
				"Krzysztof Meissner",
				new DateTime(2002, 1, 1),
				BookType.Article,
				new List<IReview>
				{
					new ExpertReview(68, "Ładnie podsumowane"),
					new ExpertReview(89, "Dobry podział tematów")
				}
			),
			new BookModel(
				"6168656cdd6f908005daabfa",
				"Pan Tadeusz",
				"Adam Mickiewicz",
				new DateTime(1834, 1, 1),
				BookType.Epic,
				new List<IReview>
				{
					new ExpertReview(99, "Kanon polskiej literatury"),
					new ExpertReview(95, "Wybitne dzieło"),
					new SimpleReview(10),
					new SimpleReview(15)
				}
			),
			new BookModel(
				"6168656cdd6f908005daabfb",
				"Na wiosnę",
				"Kazimierz Przerwa-Tetmajer",
				new DateTime(1894, 1, 1),
				BookType.Lyric,
				new List<IReview>
				{
					new ExpertReview(68, "Przyjemne"),
					new ExpertReview(89, "Łatwy język")
				}
			),
			new BookModel(
				"6168656cdd6f908005daabfc",
				"Programista 81",
				"Magazyn Programista",
				new DateTime(2019, 5, 1),
				BookType.DailyNewspaper,
				new List<IReview>
				{
					new SimpleReview(59),
					new SimpleReview(33),
					new SimpleReview(66)
				}
			),
			new BookModel(
				"6168656cdd6f908005daabfd",
				"Lalka",
				"Bolesław Prus",
				new DateTime(1890, 1, 1),
				BookType.Epic,
				new List<IReview>
				{
					new ExpertReview(90, "Jestem fanem Wokulskiego"),
					new SimpleReview(72),
					new SimpleReview(85),
					new SimpleReview(63),
					new GradeReview(Grade.C)
				}
			),
			new BookModel
			(
				"6168656cdd6f908005daabfe",
				"Hamlet",
				"William Shakespeare",
				new DateTime(1611, 1, 1),
				BookType.Drama,
				new List<IReview>
				{
					new ExpertReview(85, "Based"),
					new ExpertReview(91, "Totalnie based"),
					new SimpleReview(72),
					new SimpleReview(31),
					new SimpleReview(50)
				}
			),
			new BookModel
			(
				"6168656cdd6f908005daabff",
				"Makbet",
				"William Shakespeare",
				new DateTime(1606, 1, 1),
				BookType.Drama,
				new List<IReview>
				{
					new ExpertReview(71, "Hamlet lepszy"),
					new ExpertReview(60, "Trudny język"),
					new SimpleReview(56),
				}
			)
		};
	}
}