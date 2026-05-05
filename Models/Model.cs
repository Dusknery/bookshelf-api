using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
       : base(options)
    {
    }
    public DbSet<Book> Books { get; set; }
    public DbSet<Citation> Citations { get; set; }
    public DbSet<User> Users { get; set; }

}

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Author { get; set; } = "";
    public string PublicationDate { get; set; } = "";

    public int UserId { get; set; }
}


public class Citation
{
    public int Id { get; set; }
    public required string CitationText { get; set; }

    public int UserId { get; set; }
}
public class User
{
    public int Id { get; set; }
    public required string Email { get; set; } = "";
    public required string PasswordHash { get; set; } = "";

    public List<Book> Books { get; set; } = new();
    public List<Citation> Citations { get; set; } = new();
}