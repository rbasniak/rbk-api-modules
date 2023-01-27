using MediatR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo1.BusinessLogic.Commands;

public class EventHappenedHandler1 : INotificationHandler<EventHappened>
{
    public async Task Handle(EventHappened notification, CancellationToken cancellationToken)
    {
        Debug.WriteLine($"*** {DateTime.Now}: Handling 'EventHappened' with 'EventHappenedHandler1'");

        await Task.Delay(15000);

        Debug.WriteLine($"*** {DateTime.Now}: It happened again, I hope you enjoyed, this was the last one");
    }
}

public class EventHappenedHandler2 : INotificationHandler<EventHappened>
{
    public async Task Handle(EventHappened notification, CancellationToken cancellationToken)
    {
        Debug.WriteLine($"*** {DateTime.Now}: Handling 'EventHappened' with 'EventHappenedHandler2'");

        await Task.Delay(5000);

        Debug.WriteLine($"*** I{DateTime.Now}: t happened, let's wait for the next");
    }
}


public class EventHappened: INotification
{

}