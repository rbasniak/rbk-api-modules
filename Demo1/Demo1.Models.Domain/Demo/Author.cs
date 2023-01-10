using rbkApiModules.Commons.Core;
using System.ComponentModel.DataAnnotations;

namespace Demo1.Models.Domain.Demo;

public class Author : BaseEntity
{
    private HashSet<Post> _posts;

    protected Author()
    {

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