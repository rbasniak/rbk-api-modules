using Microsoft.Extensions.Logging;

namespace Demo1.BusinessLogic.Commands;

public interface IService4
{
    void Run();
}

public class Service4 : IService4
{
    private readonly ILogger<Service4> _logger;
    private readonly IService3 _service3;

    public Service4(IService3 service3, ILogger<Service4> logger)
    {
        _service3 = service3;
        _logger = logger;
    }

    public void Run()
    {
        _logger.LogInformation($"Started {this.GetType().Name}.{nameof(Run)}()");
        Thread.Sleep(5000);
        _service3.Run();
        _logger.LogInformation($"Finished {this.GetType().Name}.{nameof(Run)}()");
    }
}
