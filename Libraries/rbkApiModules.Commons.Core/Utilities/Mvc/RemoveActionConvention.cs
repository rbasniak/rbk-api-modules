using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace rbkApiModules.Commons.Core;

public class RemoveActionConvention : IApplicationModelConvention
{
    private readonly Tuple<Type, string>[] _actionsToRemove;

    public RemoveActionConvention(Tuple<Type, string>[] actionsToRemove)
    {
        _actionsToRemove = actionsToRemove;
    }

    public void Apply(ApplicationModel application)
    {
        foreach (var controller in application.Controllers)
        {
            var toBeRemoved = new List<ActionModel>();
            foreach (var action in controller.Actions)
            {
                foreach (var removeData in _actionsToRemove)
                {
                    if (action.Controller.ControllerType == removeData.Item1 && action.ActionName == removeData.Item2)
                    {
                        toBeRemoved.Add(action);
                    }
                }
            }

            foreach (var action in toBeRemoved)
            {
                controller.Actions.Remove(action);
            }
        }
    }
}