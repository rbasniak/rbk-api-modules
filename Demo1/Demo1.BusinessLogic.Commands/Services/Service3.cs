using Microsoft.Extensions.Logging;

namespace Demo1.BusinessLogic.Commands;

public interface IService3
{
    void Run();
}

public class Service3 : IService3
{
    private readonly ILogger<Service3> _logger;
    private readonly IService2 _service2;

    public Service3(IService2 service2, ILogger<Service3> logger)
    {
        _service2 = service2;
        _logger = logger;
    }

    public void Run()
    {
        _logger.LogInformation($"Started {this.GetType().Name}.{nameof(Run)}()");
        Thread.Sleep(3000);
        _service2.Run();
        _logger.LogInformation($"Finished {this.GetType().Name}.{nameof(Run)}()");
    }
}
