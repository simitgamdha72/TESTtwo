using System.ComponentModel.DataAnnotations;

namespace BookManagement.Models;

public class Books
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Author { get; set; } = null!;

    public DateTime PublieshedYear { get; set; }

    public string ISBN { get; set; } = null!;

    public string status { get; set; } = null!;

    public virtual ICollection<UserIssuedBooks> userIssuedBooks { get; set; } = new List<UserIssuedBooks>();
}