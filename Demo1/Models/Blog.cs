namespace Demo1.Models;

public class Blog : TenantEntity
{
    private HashSet<Post> _posts;

    private Blog()
    {
        _posts = default!;
    }

    public Blog(string tenant, string title)
    {
        _posts = [];

        TenantId = tenant;
        Title = title;
        Description = string.Empty;
    }

    [Required]
    [MinLength(2), MaxLength(128)]
    public string Title { get; private set; }

    [MinLength(16), MaxLength(1024)]
    public string Description { get; private set; }

    public IEnumerable<Post> Posts => _posts?.AsReadOnly()!;
}