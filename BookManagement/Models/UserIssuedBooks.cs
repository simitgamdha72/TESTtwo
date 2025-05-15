namespace BookManagement.Models;

public class UserIssuedBooks
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int BookId { get; set; }

    public bool isReturn { get; set; } = false;

    public virtual User User { get; set; } = null!;

    public virtual Books Books { get; set; } = null!;


}