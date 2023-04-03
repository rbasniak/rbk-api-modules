using Serilog;
using System.Text;

namespace rbkApiModules.Commons.Core.CodeGeneration;

public class NgxsStore
{
    public NgxsStore(ControllerInfo controller)
    {
        Name = controller.Name;
        Controller = controller;
        ActionsFilepath = $"{CodeGenerationUtilities.ToTypeScriptFileCase(controller.Name)}.actions";
        SelectorFilepath = $"{CodeGenerationUtilities.ToTypeScriptFileCase(controller.Name)}.selectors";
        StateFilepath = $"{CodeGenerationUtilities.ToTypeScriptFileCase(controller.Name)}.state";

        GenerateActions();
    }

    private void GenerateActions()
    {
        Log.Information("Generating actions for store: {stpre}", Name);

        Actions = new NgxsFile<NgxsAction>();

        NgxsAction clearAction = null;

        var listEndpoint = Controller.Endpoints.FirstOrDefault(x => String.IsNullOrEmpty(x.Route) && x.Method == HttpMethod.Get);
        if (listEndpoint != null && listEndpoint.IncludeInStatesGenertation)
        {
            if (listEndpoint.InputType == null)
            {
                Log.Information("Trying to add null model to action models: {method} {action} ({name})", listEndpoint.Method, listEndpoint.Route, listEndpoint.Name);
            }

            Actions.Items.Add(new NgxsAction(Controller.Name, ActionType.LoadAll, listEndpoint.InputType, listEndpoint));
            clearAction = new NgxsAction(Controller.Name, ActionType.Clear, null, null);
        }

        var createEndpoint = Controller.Endpoints.FirstOrDefault(x => String.IsNullOrEmpty(x.Route) && x.Method == HttpMethod.Post);
        if (createEndpoint != null && createEndpoint.IncludeInStatesGenertation)
        {
            if (createEndpoint.InputType == null)
            {
                Log.Information("Trying to add null model to action models: {method} {action} ({name})", createEndpoint.Method, createEndpoint.Route, createEndpoint.Name);
            }

            Actions.ModelsToImport.Add(createEndpoint.InputType);
            Actions.Items.Add(new NgxsAction(Controller.Name, ActionType.Create, createEndpoint.InputType, createEndpoint));
        }

        var updateEndpoint = Controller.Endpoints.FirstOrDefault(x => String.IsNullOrEmpty(x.Route) && x.Method == HttpMethod.Put);
        if (updateEndpoint != null && updateEndpoint.IncludeInStatesGenertation)
        {
            if (updateEndpoint.InputType == null)
            {
                Log.Information("Trying to add null model to action models: {method} {action} ({name})", updateEndpoint.Method, updateEndpoint.Route, updateEndpoint.Name);
            }

            Actions.ModelsToImport.Add(updateEndpoint.InputType);
            Actions.Items.Add(new NgxsAction(Controller.Name, ActionType.Update, updateEndpoint.InputType, updateEndpoint));
        }

        var deleteEndpoint = Controller.Endpoints.FirstOrDefault(x => x.Route != null && x.Route == "{id}" && x.Method == HttpMethod.Delete);
        if (deleteEndpoint != null && deleteEndpoint.IncludeInStatesGenertation)
        {
            if (deleteEndpoint.InputType != null)
            {
                Actions.ModelsToImport.Add(deleteEndpoint.InputType);
                Actions.Items.Add(new NgxsAction(Controller.Name, ActionType.Delete, deleteEndpoint.InputType, deleteEndpoint));
            }
            else if (deleteEndpoint.UrlParameters.Count > 0)
            {
                Actions.Items.Add(new NgxsAction(Controller.Name, ActionType.Delete, new TypeInfo(typeof(string)), deleteEndpoint));
            }
        }

        if (clearAction != null)
        {
            Actions.Items.Add(clearAction);
        }
    }

    public string Name { get; set; }
    public ControllerInfo Controller { get; set; }
    public NgxsFile<NgxsAction> Actions { get; set; }
    public string ActionsFilepath { get; set; }
    public string SelectorFilepath { get; set; }
    public string StateFilepath { get; set; }

    public string GenerateStateFile()
    {
        var code = new StringBuilder();

        var listAction = Actions.Items.FirstOrDefault(x => x.Type == ActionType.LoadAll);


        code.AppendLine($"import {{ State, Action, StateContext }} from '@ngxs/store';");
        code.AppendLine($"import {{ Observable }} from 'rxjs';");
        code.AppendLine($"import {{ tap }} from 'rxjs/operators';");
        code.AppendLine($"import {{ Injectable }} from '@angular/core';");
        code.AppendLine($"import {{ ToastActions }} from 'ngx-smz-ui';");
        code.AppendLine($"import {{ {Name}Actions }} from './{CodeGenerationUtilities.ToTypeScriptFileCase(Name)}.actions';");
        code.AppendLine($"import {{ {Name}Service }} from '@services/api/{CodeGenerationUtilities.ToTypeScriptFileCase(Name)}.service';");

        var uniqueModels = Actions.Items.Where(x => x.Endpoint != null && x.Endpoint.ReturnType != null).Select(x => x.Endpoint.ReturnType.Name).ToList();

        if (Actions.Items.Any(x => x.Type == ActionType.Update && x.Endpoint.ReturnType.Name != nameof(TreeNode)))
        {
            code.AppendLine("import { replaceItem } from 'ngx-smz-ui';");
        }

        uniqueModels = uniqueModels.DistinctBy(x => x).ToList();

        foreach (var model in uniqueModels)
        {
            if (model == nameof(SimpleNamedEntity))
            {
                code.AppendLine($"import {{ SimpleNamedEntity }} from 'ngx-smz-ui';");
            }
            else if (model == "TreeNode")
            {
                code.AppendLine($"import {{ TreeNode }} from 'primeng/api';");
            }
            else
            {
                code.AppendLine($"import {{ {model} }} from '@models/{CodeGenerationUtilities.ToTypeScriptFileCase(model)}';");
            }
        }

        code.AppendLine($"");

        code.AppendLine($"export const {CodeGenerationUtilities.ToTypeScriptFileCase(Name).ToUpper().Replace("-", "_")}_STATE_NAME = '{CodeGenerationUtilities.ToCamelCase(Name)}';");
        code.AppendLine($"");

        code.AppendLine($"export interface {Name}StateModel {{");
        code.AppendLine($"  items: {listAction.Endpoint.ReturnType.Name}[];");
        code.AppendLine($"  lastUpdated?: Date;");
        code.AppendLine($"}}");
        code.AppendLine($"");

        code.AppendLine($"@State<{Name}StateModel>({{");
        code.AppendLine($"  name: {CodeGenerationUtilities.ToTypeScriptFileCase(Name).ToUpper().Replace("-", "_")}_STATE_NAME,");
        code.AppendLine($"  defaults: {{");
        code.AppendLine($"    items: [],");
        code.AppendLine($"    lastUpdated: null");
        code.AppendLine($"  }}");
        code.AppendLine($"}})");
        code.AppendLine($"");

        code.AppendLine($"@Injectable()");
        code.AppendLine($"export class {Name}State {{");
        code.AppendLine($"  constructor(private apiService: {Name}Service) {{ }}");

        code.AppendLine($"");
        code.Append(GenerateHandlerCode(listAction));
        code.Append(GenerateHandlerCode(Actions.Items.FirstOrDefault(x => x.Type == ActionType.Create)));
        code.Append(GenerateHandlerCode(Actions.Items.FirstOrDefault(x => x.Type == ActionType.Update)));
        code.Append(GenerateHandlerCode(Actions.Items.FirstOrDefault(x => x.Type == ActionType.Delete)));

        code.Append(GenerateClearHandlerCode(Actions.Items.FirstOrDefault(x => x.Type == ActionType.Clear)));

        code.AppendLine($"}}");

        return code.ToString();
    }

    public string GenerateActionFile()
    {
        var code = new StringBuilder();

        foreach (var model in Actions.ModelsToImport)
        {
            code.AppendLine($"import {{ {model.Name} }} from '@models/{CodeGenerationUtilities.ToTypeScriptFileCase(model.Name)}';");
        }

        if (Actions.ModelsToImport.Count > 0)
        {
            code.AppendLine();
        }

        code.AppendLine($"export namespace {Controller.Name}Actions {{");

        for (var i = 0; i < Actions.Items.Count; i++)
        {
            code.AppendLine(Actions.Items[i].GenerateCode());

            if (i != Actions.Items.Count - 1)
            {
                code.AppendLine();
            }
        }

        code.AppendLine($"}}");

        return code.ToString();
    }

    public string GenerateSelectorFile()
    {
        var code = new StringBuilder();

        var listAction = Actions.Items.FirstOrDefault(x => x.Type == ActionType.LoadAll);

        code.AppendLine($"import {{ createSelector, Selector }} from '@ngxs/store';");
        code.AppendLine($"import {{ {Name}State, {Name}StateModel }} from './{CodeGenerationUtilities.ToTypeScriptFileCase(Name)}.state';");

        if (listAction.Endpoint.ReturnType.Name == nameof(SimpleNamedEntity))
        {
            code.AppendLine($"import {{ SimpleNamedEntity }} from 'ngx-smz-ui';");
        }
        else if (listAction.Endpoint.ReturnType.Name == "TreeNode")
        {
            code.AppendLine($"import {{ TreeNode }} from 'primeng/api';");
        }
        else
        {
            code.AppendLine($"import {{ {listAction.Endpoint.ReturnType.Name} }} from '@models/{CodeGenerationUtilities.ToTypeScriptFileCase(listAction.Endpoint.ReturnType.Name)}';");
        }
        code.AppendLine($"");

        code.AppendLine($"export class {Name}Selectors {{");
        code.AppendLine($"  @Selector([{Name}State])");
        code.AppendLine($"  public static all(state: {Name}StateModel): {listAction.Endpoint.ReturnType.Name}[] {{");
        code.AppendLine($"    return state.items;");
        code.AppendLine($"  }}");
        code.AppendLine($"");

        code.AppendLine($"  public static single(id: string): (state: {Name}StateModel) => {listAction.Endpoint.ReturnType.Name} {{");
        code.AppendLine($"    return createSelector([{Name}State], (state: {Name}StateModel) => id == null ? null : state.items.find(x => x.id === id));");
        code.AppendLine($"  }}");

        code.AppendLine($"}}");

        return code.ToString();
    }

    private string GenerateHandlerCode(NgxsAction action)
    {
        if (action == null) return String.Empty;

        var code = new StringBuilder();

        var returnType = "void";

        if (action.Endpoint != null && action.Endpoint.ReturnType != null)
        {
            returnType = $"{action.Endpoint.ReturnType.Name}" + (action.Endpoint.ReturnType.IsList ? "[]" : "");
        }

        var actionPayloadName = action.Type == ActionType.Delete ? "id" : "data";

        code.AppendLine($"");
        code.AppendLine($"  @Action({Name}Actions.{action.Type.ToString()})");
        code.AppendLine($"  public {CodeGenerationUtilities.ToCamelCase(action.Type.ToString())}$(ctx: StateContext<{Name}StateModel>{(action.Type != ActionType.LoadAll ? $", action: {Name}Actions.{action.Type.ToString()}" : "")}): {(action.Endpoint != null ? $"Observable<{returnType}>" : returnType)} {{");
        code.AppendLine($"    return this.apiService.{CodeGenerationUtilities.ToCamelCase(action.Endpoint.Name)}({(action.Type != ActionType.LoadAll ? $"action.{actionPayloadName}" : "")}).pipe(");
        code.AppendLine($"      tap({(action.Endpoint != null && action.Endpoint.ReturnType != null ? $"(result: {returnType})" : "()")} => {{");

        if (action.Type == ActionType.LoadAll)
        {
            code.AppendLine($"        ctx.patchState({{");
            code.AppendLine($"          lastUpdated: new Date(),");
            code.AppendLine($"          items: result,");
            code.AppendLine($"        }});");
        }

        if (action.Type == ActionType.Create)
        {
            code.AppendLine($"        ctx.dispatch(new ToastActions.Success('Criação realizada com sucesso'));");
            code.AppendLine($"        ctx.patchState({{");

            if (action.Endpoint.StoreBehavior == StoreBehavior.General)
            {
                code.AppendLine($"          items: [ result, ...ctx.getState().items ]");
            }
            else if (action.Endpoint.StoreBehavior == StoreBehavior.ReplaceAll)
            {
                code.AppendLine($"          lastUpdated: new Date(),");
                code.AppendLine($"          items: result,");
            }
            else
            {
                throw new NotSupportedException($"{action.Endpoint.StoreBehavior} not supported for {action.Endpoint.Route}");
            }
            code.AppendLine($"        }});");
        }

        if (action.Type == ActionType.Update)
        {
            code.AppendLine($"        ctx.dispatch(new ToastActions.Success('Atualização realizada com sucesso'));");
            code.AppendLine($"        ctx.patchState({{");

            if (action.Endpoint.StoreBehavior == StoreBehavior.General)
            {
                code.AppendLine($"          items: replaceItem(ctx.getState().items, result)");
            }
            else if (action.Endpoint.StoreBehavior == StoreBehavior.ReplaceAll)
            {
                code.AppendLine($"          lastUpdated: new Date(),");
                code.AppendLine($"          items: result,");
            }
            else
            {
                throw new NotSupportedException($"{action.Endpoint.StoreBehavior} not supported for {action.Endpoint.Route}");
            }
            code.AppendLine($"        }});");
        }

        if (action.Type == ActionType.Delete)
        {
            code.AppendLine($"        ctx.dispatch(new ToastActions.Success('Exclusão realizada com sucesso'));");
            code.AppendLine($"        ctx.patchState({{");

            if (action.Endpoint.StoreBehavior == StoreBehavior.General)
            {
                code.AppendLine($"          items: [ ...ctx.getState().items.filter(x => x.id !== action.id) ]");
            }
            else if (action.Endpoint.StoreBehavior == StoreBehavior.ReplaceAll)
            {
                code.AppendLine($"          lastUpdated: new Date(),");
                code.AppendLine($"          items: result,");
            }
            else
            {
                throw new NotSupportedException($"{action.Endpoint.StoreBehavior} not supported for {action.Endpoint.Route}");
            }
            code.AppendLine($"        }});");
        }

        code.AppendLine($"      }})");
        code.AppendLine($"    );");
        code.AppendLine($"  }}");

        return code.ToString();
    }

    private string GenerateClearHandlerCode(NgxsAction action)
    {
        if (action == null) return String.Empty;

        var code = new StringBuilder();

        code.AppendLine($"");
        code.AppendLine($"  @Action({Name}Actions.{action.Type.ToString()})");
        code.AppendLine($"  public {CodeGenerationUtilities.ToCamelCase(action.Type.ToString())}$(ctx: StateContext<{Name}StateModel>): void {{");
        code.AppendLine($"    ctx.patchState({{");
        code.AppendLine($"      items: [],");
        code.AppendLine($"      lastUpdated: null");
        code.AppendLine($"    }});");
        code.AppendLine($"  }}");

        return code.ToString();
    }
}

public enum ActionType
{
    LoadAll,
    Create,
    Update,
    Delete,
    Clear,
}

public class NgxsFile<T>
{
    public NgxsFile()
    {
        Items = new List<T>();
        ModelsToImport = new List<TypeInfo>();
    }

    public List<T> Items { get; set; }
    public List<TypeInfo> ModelsToImport { get; set; }
}
