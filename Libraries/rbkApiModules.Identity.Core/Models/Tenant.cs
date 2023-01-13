﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace rbkApiModules.Identity.Core;

public class Tenant 
{
    protected HashSet<User> _users;

    protected Tenant()
    {

    }

    public Tenant(string alias, string name, string metadata = null)
    {
        Alias = alias;
        Name = name;
        Metadata = metadata;

        _users = new HashSet<User>();
    }

    [Key, Required, MaxLength(32)]
    public virtual string Alias { get; protected set; }

    [Required, MinLength(3), MaxLength(255)]
    public virtual string Name { get; protected set; }

    public virtual string Metadata { get; protected set; }

    public virtual IEnumerable<User> Users => _users?.ToList();

    public void Update(string name, string metadata)
    {
        Name = name;
        Metadata = metadata;
    } 

    public virtual T GetMetadata<T>()
    {
        return JsonSerializer.Deserialize<T>(Metadata);
    }
} 