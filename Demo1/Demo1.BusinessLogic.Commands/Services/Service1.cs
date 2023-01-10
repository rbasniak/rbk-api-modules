using Microsoft.Extensions.Logging;

namespace Demo1.BusinessLogic.Commands;

public interface IService1
{
    void Run();
}

public class Service1 : IService1
{
    private readonly ILogger<Service1> _logger;

    public Service1(ILogger<Service1> logger)
    {
        _logger = logger;
    }

    public void Run()
    {
        _logger.LogInformation($"Started {this.GetType().Name}.{nameof(Run)}()");
        Thread.Sleep(1000);
        _logger.LogInformation($"Finished {this.GetType().Name}.{nameof(Run)}()");
    }
}
