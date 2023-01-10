using System.Text.Json.Serialization;

namespace rbkApiModules.Commons.Core.CQRS;

public interface IHasReadingModel<T> where T : class
{
    [JsonIgnore]
    OperationType Mode { get; }
}

public enum OperationType
{
    AddOrUpdate,
    Remove
}