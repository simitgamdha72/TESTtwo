namespace BookManagement.Models;

public class User
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Role { get; set; } = null!;

    public virtual ICollection<UserIssuedBooks> userIssuedBooks { get; set; } = new List<UserIssuedBooks>();

}