﻿using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Auditing;
using System.ComponentModel.DataAnnotations;

namespace Demo1.Models.Domain.Demo;

public class Post : TenantEntity
{
    protected Post()
    {

    }

    public Post(string tenant, Blog blog, Author author, string title, string body, DateTime? publishDate = null)
    {
        TenantId = tenant;
        Title = title;
        Body = body;
        Author = author;
        Blog = blog;
        PublishingDate = publishDate;
    }

    [Required]
    [MinLength(2), MaxLength(16)]
    public string Title { get; private set; }

    [MinLength(32), MaxLength(4096)]
    public string Body { get; private set; }

    public DateTime? PublishingDate { get; private set; }

    [Required]
    public Guid? AuthorId { get; private set; }
    public Author Author { get; private set; }

    [Required]
    public Guid BlogId { get; private set; }
    public Blog Blog { get; private set; }

    public void Update(string title, string body)
    {
        Title = title;
        Body = body;
    }
}