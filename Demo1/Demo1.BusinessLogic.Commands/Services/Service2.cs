using Microsoft.Extensions.Logging;

namespace Demo1.BusinessLogic.Commands;

public interface IService2
{
    void Run();
}

public class Service2 : IService2
{
    private readonly ILogger<Service2> _logger;
    private readonly IService1 _service1;

    public Service2(IService1 service1, ILogger<Service2> logger)
    {
        _service1 = service1;
        _logger = logger;
    }

    public void Run()
    {
        _logger.LogInformation($"Started {this.GetType().Name}.{nameof(Run)}()");
        Thread.Sleep(2000);
        _service1.Run();
        _logger.LogInformation($"Finished {this.GetType().Name}.{nameof(Run)}()");
    }
}
