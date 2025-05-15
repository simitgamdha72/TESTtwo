using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookManagement.Models;
using BookManagement.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;


namespace TestProject.Controllers;

public class BookController : Controller
{
    private readonly BookManagementContext _context;

    public BookController(BookManagementContext Context)
    {
        _context = Context;
    }

    public IActionResult ShowBookList(BookSearchViewModel bookSearchViewModel)
    {
        var query = _context.Books.AsQueryable();

        if (!string.IsNullOrEmpty(bookSearchViewModel.SearchString))
        {
            query = query.Where(q => q.Title.ToLower().Contains(bookSearchViewModel.SearchString.ToLower()) || q.Author.ToLower().Contains(bookSearchViewModel.SearchString.ToLower()) || q.ISBN.ToLower().Contains(bookSearchViewModel.SearchString.ToLower()));
        }

        var TotalItems = query.Count();

        query = bookSearchViewModel.SortDirection == "asc"
                ? query.OrderBy(o => o.Title)
                : query.OrderByDescending(o => o.Title);


        query = query
            .Skip((bookSearchViewModel.CurrentPage - 1) * bookSearchViewModel.PageSize)
            .Take(bookSearchViewModel.PageSize);


        BookViewModel bookViewModel = new BookViewModel
        {
            books = query.ToList(),
        };

        ViewBag.totalitemscount = TotalItems;
        ViewBag.pageSize = bookSearchViewModel.PageSize;
        ViewBag.currentPage = bookSearchViewModel.CurrentPage;

        return PartialView("_BookList", bookViewModel);
    }

    public IActionResult ShowIssuedBookList(BookSearchViewModel bookSearchViewModel)
    {
        var query = _context.Books.AsQueryable();

        string? cookie = Convert.ToString(User.FindFirstValue(ClaimTypes.Email));

        if (cookie == null)
        {
            return RedirectToAction("Error", "Home");
        }

        User? user = _context.Users.FirstOrDefault(u => u.Email == cookie);

        if (user == null)
        {
            return RedirectToAction("Error", "Home");
        }



        BookViewModel bookViewModel = new BookViewModel
        {
            books = query.ToList(),
        };

        return PartialView("_BookList", bookViewModel);
    }



    [HttpGet]
    public IActionResult AddNewBookModal()
    {
        return PartialView("_AddNewBook");
    }

    [HttpPost]
    public IActionResult AddNewBook(BookViewModel bookViewModel)
    {
        if (ModelState.ContainsKey("status")) ModelState.Remove("status");

        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Invalid Data" });
        }

        Books? books = _context.Books.FirstOrDefault(b => b.Title == bookViewModel.Title || b.ISBN == bookViewModel.ISBN);

        if (books != null)
        {
            return Json(new { success = false, message = "Book is Allready Exists!" });
        }

        Books NewBook = new Books
        {
            Title = bookViewModel.Title,
            Author = bookViewModel.Author,
            PublieshedYear = bookViewModel.PublieshedYear,
            ISBN = bookViewModel.ISBN,
            status = "available"

        };

        _context.Books.Add(NewBook);
        _context.SaveChanges();

        return Json(new { success = true, message = "Book is Added" });
    }

    [HttpGet]
    public IActionResult EditBookModal(int id)
    {
        Books? books = _context.Books.FirstOrDefault(b => b.Id == id);
        if (books == null)
        {
            return Json(new { success = false, });
        }

        BookViewModel bookViewModel = new BookViewModel
        {
            Id = id,
            Title = books.Title,
            Author = books.Author,
            PublieshedYear = books.PublieshedYear,
            ISBN = books.ISBN,
        };
        return PartialView("_EditBook", bookViewModel);
    }

    [HttpPost]
    public IActionResult EditBook(BookViewModel bookViewModel)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Invalid Data" });
        }

        Books? books = _context.Books.FirstOrDefault(b => b.Title == bookViewModel.Title || b.ISBN == bookViewModel.ISBN);

        if (books != null && books.Id != bookViewModel.Id)
        {
            return Json(new { success = false, message = "Book is Allready Exists!" });
        }

        Books? EditBook = _context.Books.FirstOrDefault(b => b.Id == bookViewModel.Id);

        if (EditBook == null)
        {
            return Json(new { success = false, message = "Book Not Found" });
        }

        EditBook.Author = bookViewModel.Author;
        EditBook.ISBN = bookViewModel.ISBN;
        EditBook.Title = bookViewModel.Title;
        EditBook.PublieshedYear = bookViewModel.PublieshedYear;

        _context.Books.Update(EditBook);
        _context.SaveChanges();

        return Json(new { success = true, message = "Book Edited Successfully" });

    }

    [HttpGet]
    public IActionResult DeleteBookModal(int id)
    {
        Books? books = _context.Books.FirstOrDefault(b => b.Id == id);
        if (books == null)
        {
            return Json(new { success = false, });
        }

        BookViewModel bookViewModel = new BookViewModel
        {
            Id = id,
        };
        return PartialView("_DeleteBook", bookViewModel);
    }

    [HttpPost]
    public IActionResult DeleteBook(BookViewModel bookViewModel)
    {
        Books? books = _context.Books.FirstOrDefault(b => b.Id == bookViewModel.Id);

        if (books == null)
        {
            return Json(false);
        }

        _context.Books.Remove(books);
        _context.SaveChanges();

        return Json(true);
    }

    public IActionResult IssueBook(int id)
    {
        Books? books = _context.Books.FirstOrDefault(b => b.Id == id);
        if (books == null)
        {
            return Json(new { success = false, message = "Book Not Found" });
        }

        string? cookie = Convert.ToString(User.FindFirstValue(ClaimTypes.Email));

        if (cookie == null)
        {
            return RedirectToAction("Error", "Home");
        }

        User? user = _context.Users.FirstOrDefault(u => u.Email == cookie);

        if (user == null)
        {
            return RedirectToAction("Error", "Home");
        }

        List<UserIssuedBooks> userIssuedBooksByUser = _context.UserIssuedBooks.Where(u => u.UserId == user.Id && u.isReturn != true).ToList();

        if (userIssuedBooksByUser.Count() >= 3)
        {
            return Json(new { success = false, message = "Maximum Capacity reach" });
        }

        UserIssuedBooks userIssuedBooks = new UserIssuedBooks
        {
            UserId = user.Id,
            BookId = books.Id,
        };

        _context.UserIssuedBooks.Add(userIssuedBooks);


        books.status = "issued";
        _context.Books.Update(books);

        _context.SaveChanges();

        return Json(new { success = true, message = "Book Issued to You" });
    }
}
