using Microsoft.EntityFrameworkCore;

namespace BookManagement.Models;

public class BookManagementContext : DbContext
{

    public BookManagementContext(DbContextOptions<BookManagementContext> options) : base(options)
    {

    }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Books> Books { get; set; }
    public virtual DbSet<UserIssuedBooks> UserIssuedBooks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<User>(entity =>
       {
           entity.HasKey(e => e.Id).HasName("User_pkey");

           entity.HasIndex(e => e.Email, "unique_email").IsUnique();

           entity.HasIndex(e => e.UserName, "unique_username").IsUnique();

           entity.Property(e => e.Email)
                          .HasColumnType("character varying")
                          .HasColumnName("email");

           entity.Property(e => e.UserName)
                            .HasMaxLength(500)
                            .HasColumnType("character varying")
                            .HasColumnName("user_name");

           entity.Property(e => e.Password)
                   .HasMaxLength(100)
                   .HasColumnName("password");

           entity.Property(e => e.Role)
                 .HasColumnType("character varying")
                 .HasColumnName("role");

       });

        modelBuilder.Entity<Books>(entity =>
       {
           entity.HasKey(e => e.Id).HasName("Books_pkey");

           entity.HasIndex(e => e.ISBN, "unique_ISBN").IsUnique();

           entity.HasIndex(e => e.Title, "unique_email").IsUnique();

           entity.Property(e => e.Title)
                            .HasMaxLength(500)
                            .HasColumnType("character varying")
                            .HasColumnName("Title");

           entity.Property(e => e.Author)
          .HasMaxLength(500)
          .HasColumnType("character varying")
          .HasColumnName("Author");

           entity.Property(e => e.status)
                .HasColumnType("character varying")
                .HasColumnName("status");



       });

        modelBuilder.Entity<UserIssuedBooks>(entity =>
       {
           entity.HasKey(e => e.Id).HasName("UserIssuedBooks_pkey");

           entity.HasOne(d => d.User).WithMany(p => p.userIssuedBooks)
                          .HasForeignKey(d => d.Id)
                          .HasConstraintName("UserIssuedBooks_userid_by_fkey");

           entity.HasOne(d => d.Books).WithMany(p => p.userIssuedBooks)
                                     .HasForeignKey(d => d.Id)
                                     .HasConstraintName("UserIssuedBooks_bookid_by_fkey");

           entity.Property(e => e.isReturn).HasColumnName("isReturn");
       });

    }
}