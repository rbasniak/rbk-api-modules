namespace Demo6.Processing;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();

        app.MapGet("/process", () => "Processing completed successfully");

        app.Run();
    }
}