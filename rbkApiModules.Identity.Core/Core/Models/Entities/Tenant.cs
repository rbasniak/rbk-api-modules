using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace rbkApiModules.Identity.Core;

public sealed class Tenant 
{
    private Tenant()
    {

    }

    public Tenant(string alias, string name, string? metadata = null)
    {
        Alias = alias.ToUpper();
        Name = name;
        Metadata = metadata;
    }

    [Key, Required, MaxLength(255)]
    public string Alias { get; private set; } = string.Empty;

    [Required, MinLength(3), MaxLength(255)]
    public string Name { get; private set; } = string.Empty;

    public string? Metadata { get; private set; } = string.Empty;

    public void Update(string name, string metadata)
    {
        Name = name;
        Metadata = metadata;
    } 

    public T GetMetadata<T>()
    {
        if (string.IsNullOrEmpty(Metadata)) throw new InvalidOperationException("Metadata is not set");

        var result = JsonSerializer.Deserialize<T>(Metadata);

        if (result == null) throw new InvalidOperationException("Metadata deserialization failed");

        return result;
    }
} 