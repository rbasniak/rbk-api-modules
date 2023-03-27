namespace Demo3;

public class Program
{
    public static int Main(string[] args)
    {
        try
        {
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .Build()
                .Run();

            return 0;
        }
        catch (Exception ex)
        {
            return 1;
        }
        finally
        {
        }
    }
}