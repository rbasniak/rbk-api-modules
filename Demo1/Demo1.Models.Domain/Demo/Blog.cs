using rbkApiModules.Commons.Core;
using System.ComponentModel.DataAnnotations;

namespace Demo1.Models.Domain.Demo;

public class Blog : TenantEntity
{
    private HashSet<Post> _posts;

    protected Blog()
    {

    }

    public Blog(string tenant, string title)
    {
        _posts = new HashSet<Post>();

        TenantId = tenant;
        Title = title;
        Description = string.Empty;
    }

    [Required]
    [MinLength(2), MaxLength(128)]
    public string Title { get; private set; }

    [MinLength(16), MaxLength(1024)]
    public string Description { get; private set; }

    public IEnumerable<Post> Posts => _posts?.ToList();
}