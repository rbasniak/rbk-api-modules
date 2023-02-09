using Demo2.Domain.Events;
using Demo2.Domain.Events.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using System;

namespace Demo2.Domain.Models;

public class Fic
{
    public Fic(string number, string name, string source)
    {
        Id = Guid.NewGuid();
        Name = name;
        Source = source;
        Number = number;
    }

    public Guid Id { get; protected set; }
    public string Number { get; protected set; }
    public string Name { get; protected set; }
    public string Source { get; protected set; }
} 