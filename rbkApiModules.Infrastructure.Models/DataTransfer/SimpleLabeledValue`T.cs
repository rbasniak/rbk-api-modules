namespace rbkApiModules.Infrastructure.Models
{
    public class SimpleLabeledValue<T>
    {
        public SimpleLabeledValue(string label, T value)
        {
            Label = label;
            Value = value;
        }

        public string Label { get; set; }
        public T Value { get; set; }
    }
}
