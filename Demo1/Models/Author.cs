namespace Demo1.Models;

public class Author : BaseEntity
{
    private HashSet<Post> _posts;

    private Author()
    {
        _posts = default!; // For EF Core
    }

    public Author(string name)
    {
        _posts = new HashSet<Post>();

        Name = name;
    }

    [Required]
    [MinLength(2), MaxLength(128)]
    public string Name { get; private set; }

    public IEnumerable<Post> Posts => _posts?.ToList();
}