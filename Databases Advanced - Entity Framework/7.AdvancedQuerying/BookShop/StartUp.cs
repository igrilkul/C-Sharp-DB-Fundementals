namespace BookShop
{
    using BookShop.Models;
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;
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
                System.Console.WriteLine(GetMostRecentBooks(db));
            }
        }

        public static string GetBooksByAgeRestriction(BookShopContext context,
string command)
        {
            var ageRestriction = Enum.Parse<AgeRestriction>(command,true);
            var books = context.Books
                .Where(b => b.AgeRestriction == ageRestriction)
                .Select(t=>t.Title)
                .OrderBy(x => x)
                .ToList();
           
            var result = string.Join(Environment.NewLine, books);
            return result;
        }

        public static string GetGoldenBooks(BookShopContext context)
        {
           List<Book> books= context.Books.Where(b => b.EditionType == Models.Enums.EditionType.Gold).Where(b => b.Copies < 5000).OrderBy(x => x.BookId).ToList();

            StringBuilder sb = new StringBuilder();
            foreach(var book in books)
            {
                sb.AppendLine($"{book.Title}");
            }
            return sb.ToString().TrimEnd();
        }

        public static string GetBooksByPrice(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            context.Books
                .Where(b => b.Price > 40)
                .Select(b => new
                {
                    b.Title,
                    b.Price
                })
                .OrderByDescending(b => b.Price)
                .ToList()
                .ForEach(b => sb.AppendLine($"{b.Title} - ${b.Price:f2}"));

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksNotReleasedIn(BookShopContext context, int
year)
        {
            var books = context.Books
                .Where(b => b.ReleaseDate.Value.Year != year)
                .OrderBy(h=>h.BookId)
                .Select(t => t.Title)
                .ToList();

            return string.Join(Environment.NewLine,books);
        }

        public static string GetBooksByCategory(BookShopContext context, string
input)
        {
            string[] categories = input.ToLower().Split(" ",StringSplitOptions.RemoveEmptyEntries).ToArray();
            var books = context.Books
                .Where(x => x.BookCategories.Any(y => categories.Contains(y.Category.Name.ToLower())))
                .Select(t => t.Title)
                .OrderBy(h=>h)
                .ToList();

            return string.Join(Environment.NewLine, books);
        }

        public static string GetBooksReleasedBefore(BookShopContext context,
string date)
        {
            var targetDate = DateTime.ParseExact(date,"dd-MM-yyyy",null);

            var books = context.Books
                .Where(b => b.ReleaseDate < targetDate)
                .OrderByDescending(y => y.ReleaseDate)
                .Select(t => new { t.Title, t.EditionType, t.Price})
                .ToList();

            StringBuilder sb = new StringBuilder();
            foreach(var book in books)
            {
                sb.AppendLine($"{book.Title} - {book.EditionType} - ${book.Price:F2}");
            }
            return sb.ToString().TrimEnd();
        }

        public static string GetAuthorNamesEndingIn(BookShopContext context,
string input)
        {
            input = input.ToLower();

            var authors = context.Authors
                .Where(a => a.FirstName.ToLower().EndsWith(input))
                .Select(t => new { FullName = (t.FirstName + " " + t.LastName)})
                .OrderBy(t=>t.FullName)
                .ToList();

            StringBuilder sb = new StringBuilder();
            foreach(var a in authors)
            {
                sb.AppendLine(a.FullName);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBookTitlesContaining(BookShopContext context, string
input)
        {
            input = input.ToLower();

            var books = context.Books
                .Where(t => t.Title.ToLower().Contains(input))
                .Select(y => y.Title)
                .OrderBy(x => x)
                .ToList();

            return string.Join(Environment.NewLine, books);
        }

        public static string GetBooksByAuthor(BookShopContext context, string
input)
        {
            input = input.ToLower();
            var booksAndAuthors = context.Books
                .Where(b => b.Author.LastName.ToLower().StartsWith(input))
                .OrderBy(x=>x.BookId)
                .Select(t => new { Description = (t.Title + " (" + t.Author.FirstName + " " + t.Author.LastName + ")") })
                .ToList();

            StringBuilder sb = new StringBuilder();
            foreach(var ba in booksAndAuthors)
            {
                sb.AppendLine(ba.Description);
            }

            return sb.ToString().TrimEnd();
        }

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            int booksCount = context.Books
                .Where(t => t.Title.Length > lengthCheck).Count();

            return booksCount;
        }

        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var authors = context.Authors
                .OrderByDescending(y=>y.Books.Sum(u=>u.Copies))
                .Select(x => new { Description = (x.FirstName + " " + x.LastName + " - " + x.Books.Sum(u=>u.Copies))})
                .ToList();

            StringBuilder sb = new StringBuilder();
            foreach(var a in authors)
            {
                sb.AppendLine(a.Description);
            }
            return sb.ToString().TrimEnd();
        }

        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var categoriesProfits = context.Categories
                .Select(x => new
                {
                    TotalProfit = (
                    x.CategoryBooks.Sum(y => y.Book.Price * y.Book.Copies)),
                    x.Name
                })
                .OrderByDescending(t=>t.TotalProfit)
                .ThenBy(i=>i.Name)
                .ToList();

            StringBuilder sb = new StringBuilder();
            foreach(var cp in categoriesProfits)
            {
                sb.AppendLine($"{cp.Name} ${cp.TotalProfit:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetMostRecentBooks(BookShopContext context)
        {
            var categoryBooksRecent = context.Categories
                .OrderBy(c => c.Name)
                .Select(x => new
                {
                    x.Name,
                    Books = x.CategoryBooks
                    .Select(y => new
                    {
                        y.Book.Title,
                        y.Book.ReleaseDate
                    }).OrderByDescending(h=>h.ReleaseDate)
                    .Take(3)
                    .ToList()
                }).ToList();

            StringBuilder sb = new StringBuilder();
            foreach(var cbr in categoryBooksRecent)
            {
                sb.AppendLine($"--{cbr.Name}");
                    foreach(var book in cbr.Books)
                {
                    sb.AppendLine($"{book.Title} ({book.ReleaseDate.Value.Year})");
                }
            }
            return sb.ToString().TrimEnd();
                
        }

        public static void IncreasePrices(BookShopContext context)
        {
            var books = context.Books
                .Where(b => b.ReleaseDate.Value.Year < 2015)
                .ToList();

            foreach(var book in books)
            {
                book.Price += 5;
            }
            context.SaveChanges();
        }

        public static int RemoveBooks(BookShopContext context)
        {
          var books =  context.Books
                .Where(x => x.Copies < 4200)
                .ToList();

            context.RemoveRange(books);
            context.SaveChanges();
            return books.Count();
        }
    }

    
}
