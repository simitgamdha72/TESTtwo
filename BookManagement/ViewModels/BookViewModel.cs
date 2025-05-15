using System.ComponentModel.DataAnnotations;
using BookManagement.Models;

namespace BookManagement.ViewModels;

public class BookViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Title is required")]
    [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "Title cannot be only spaces.")]
    public string Title { get; set; } = null!;

    [Required(ErrorMessage = "Author is required")]
    [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "Author cannot be only spaces.")]
    public string Author { get; set; } = null!;

    [Required]
    public DateTime PublieshedYear { get; set; }

    [Required(ErrorMessage = "ISBN is required")]
    [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "ISBN cannot be only spaces.")]
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