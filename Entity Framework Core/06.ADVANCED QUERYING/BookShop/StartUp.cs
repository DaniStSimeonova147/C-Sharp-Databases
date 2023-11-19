namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using (var db = new BookShopContext())
            {
                //DbInitializer.ResetDatabase(db);

                Console.WriteLine(GetMostRecentBooks(db));
            }
        }
        //P01
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            var age = Enum.Parse<AgeRestriction>(command, true);

            var books = context.Books
                .Select(b => new
                {
                    b.Title,
                    b.AgeRestriction
                })
                .Where(b => b.AgeRestriction == age)
                .OrderBy(b => b.Title)
                .ToList();

            StringBuilder result = new StringBuilder();

            foreach (var book in books)
            {
                result.AppendLine(book.Title);
            }

            return result.ToString().TrimEnd();
        }

        //P02
        public static string GetGoldenBooks(BookShopContext context)
        {
            var books = context.Books
                .Select(b => new
                {
                    b.Title,
                    b.BookId,
                    b.Copies,
                    b.EditionType
                })
                .Where(b => b.EditionType == EditionType.Gold && b.Copies < 5000)
                .OrderBy(b => b.BookId)
                .ToList();

            StringBuilder result = new StringBuilder();

            foreach (var book in books)
            {
                result.AppendLine(book.Title);
            }

            return result.ToString().TrimEnd();
        }

        //P03
        public static string GetBooksByPrice(BookShopContext context)
        {
            var books = context.Books
                .Select(b => new
                {
                    b.Price,
                    b.Title
                })
                .Where(b => b.Price > 40)
                .OrderByDescending(b => b.Price)
                .ToList();

            StringBuilder result = new StringBuilder();

            foreach (var book in books)
            {
                result.AppendLine($"{book.Title} - ${book.Price:f2}");
            }

            return result.ToString().TrimEnd();
        }

        //P04
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var books = context.Books
                .Select(b => new
                {
                    b.ReleaseDate,
                    b.BookId,
                    b.Title
                })
                .Where(b => b.ReleaseDate.Value.Year != year)
                .OrderBy(b => b.BookId)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine(book.Title);
            }

            return sb.ToString().TrimEnd();
        }

        //P05
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            string[] categories = input
            .Split(" ", StringSplitOptions.RemoveEmptyEntries)
            .ToArray();

            List<string> bookTitles = new List<string>();

            foreach (var category in categories)
            {
                var books = context.Books
                    .Where(b => b.BookCategories
                    .Any(c => c.Category.Name.ToLower() == category.ToLower()))
                    .Select(b => new
                    {
                        b.Title
                    })
                    .ToList();

                foreach (var book in books)
                {
                    bookTitles.Add(book.Title);
                }
            }

            StringBuilder result = new StringBuilder();

            foreach (var title in bookTitles.OrderBy(t => t))
            {
                result.AppendLine(title);
            }

            return result.ToString().TrimEnd();
        }

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            DateTime dateTime = DateTime.ParseExact(date, "dd-MM-yyyy",
            System.Globalization.CultureInfo.InvariantCulture);

            var books = context.Books
                .Where(b => b.ReleaseDate < dateTime)
                .OrderByDescending(b => b.ReleaseDate)
                .Select(b => new
                {
                    b.Title,
                    b.EditionType,
                    b.Price
                })
                .ToList();

            StringBuilder result = new StringBuilder();

            foreach (var book in books)
            {
                result.AppendLine($"{book.Title} - {book.EditionType} - ${book.Price:F2}");
            }

            return result.ToString().TrimEnd();
        }

        //P07
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authors = context.Authors
                .Where(a => a.FirstName.EndsWith(input))
                .Select(a => new
                {
                    FullName = $"{a.FirstName} {a.LastName}"
                })
                .OrderBy(a => a.FullName)
                .ToList();

            StringBuilder result = new StringBuilder();

            foreach (var author in authors)
            {
                result.AppendLine(author.FullName);
            }

            return result.ToString().TrimEnd();
        }

        //P08
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var books = context.Books
                .Select(b => new
                {
                    b.Title
                })
                .Where(b => b.Title.ToLower().Contains(input.ToLower()))
                .OrderBy(b => b.Title)
                .ToList();

            StringBuilder result = new StringBuilder();

            foreach (var book in books)
            {
                result.AppendLine(book.Title);
            }

            return result.ToString().TrimEnd();
        }

        //P09
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var books = context.Books
                .Where(b => b.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .OrderBy(b => b.BookId)
                .Select(b => new
                {
                    b.Title,
                    Author = $"{b.Author.FirstName} {b.Author.LastName}"
                })
                .ToList();

            StringBuilder result = new StringBuilder();

            foreach (var book in books)
            {
                result.AppendLine($"{book.Title} ({book.Author})");
            }

            return result.ToString().TrimEnd();
        }

        //P10
        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            return context.Books
                .Where(b => b.Title.Length > lengthCheck)
                .Count();
        }

        //P11
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var authors = context.Authors
                .Select(a => new
                {
                    Author = $"{a.FirstName} {a.LastName}",
                    BookCopies = a.Books.Sum(b => b.Copies)
                })
                .OrderByDescending(b => b.BookCopies)
                .ToList();

            StringBuilder result = new StringBuilder();

            foreach (var author in authors)
            {
                result.AppendLine($"{author.Author} - {author.BookCopies}");
            }

            return result.ToString().TrimEnd();
        }

        //P12
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var categories = context.Categories
                .Select(c => new
                {
                    c.Name,
                    TotalMoney = c.CategoryBooks.Sum(cb => cb.Book.Price * cb.Book.Copies)
                })
                .OrderByDescending(c => c.TotalMoney)
                .ThenBy(c => c.Name)
                .ToList();


            StringBuilder result = new StringBuilder();

            foreach (var category in categories)
            {
                result.AppendLine($"{category.Name} ${category.TotalMoney:f2}");
            }

            return result.ToString().TrimEnd();
        }

        //P13
        public static string GetMostRecentBooks(BookShopContext context)
        {
            var categories = context.Categories
                .OrderBy(c => c.Name)
                .Select(c => new
                {
                    CaregoryName = c.Name,
                    books = c.CategoryBooks
                              .Select(cb => new
                              {
                                  BookTitle = cb.Book.Title,
                                  BookReleaseDate = cb.Book.ReleaseDate,
                                  BookReleaseYear = cb.Book.ReleaseDate.Value.Year
                              })
                              .OrderByDescending(b => b.BookReleaseDate)
                              .Take(3)
                              .ToList()
                })
                .ToList();



            StringBuilder result = new StringBuilder();

            foreach (var category in categories)
            {
                result.AppendLine($"--{category.CaregoryName}");

                foreach (var book in category.books)
                {
                    result.AppendLine($"{book.BookTitle} ({book.BookReleaseYear})");
                }
            }

            return result.ToString().TrimEnd();
        }

        //P14
        public static void IncreasePrices(BookShopContext context)
        {
            var books = context.Books.Where(b => b.ReleaseDate.Value.Year < 2010);
                
            foreach (var book in books)
            {
                book.Price += 5;
            }

            context.SaveChanges();
        }

        //P15
        public static int RemoveBooks(BookShopContext context)
        {
            var books = context.Books.Where(b => b.Copies < 4200);
            int count = books.Count();

            foreach (var book in books)
            {
                context.Books.Remove(book);
            }

            context.SaveChanges();

            return count;
        }
    }
}
