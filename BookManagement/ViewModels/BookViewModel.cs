using System.ComponentModel.DataAnnotations;
using BookManagement.Models;

namespace BookManagement.ViewModels;

public class BookViewModel
{
    public int? Id { get; set; }

    public string Title { get; set; } = null!;

    public string Author { get; set; } = null!;

    public DateTime PublieshedYear { get; set; }

    public string ISBN { get; set; } = null!;

    public string status { get; set; } = "available";

    public List<Books>? books { get; set; }

}

public class BookSearchViewModel
{
    public string? SearchString { get; set; }
    public int PageSize { get; set; } = 5;
    public int CurrentPage { get; set; } = 1;
    public string? SortDirection { get; set; }
}