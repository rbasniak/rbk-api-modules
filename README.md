[![Build Status](https://img.shields.io/appveyor/ci/thiagoloureiro/netcore-jwt-integrator-extension/master.svg)](https://ci.appveyor.com/project/thiagoloureiro/netcore-jwt-integrator-extension)

# 1. Introduction

These are a set of libraries to help scaffold production ready APIs in ASP.NET Core, using SQL Server or MongoDB. They can be used individually, but to take most benefit of all features, it should be used with [ngx-rbk-utils](https://github.com/rbasniak/ngx-rbk-utils) and [ngx-smz-dialogs](https://github.com/smarza/ngx-smz). These two requires Angular and PrimeNG for the frontend part. If this is not suitable for you, the API libraries can be used as well, but some feature won't be available.

The libraries must always be used in the exact same version and the package is composed by the following libraries:

* Required libraries, contains all basic API infrastructure needed:
  * `rbkApiModules.Infrastructure.Api`
  * `rbkApiModules.Infrastructure.MediatR.Core`
  * `rbkApiModules.Infrastructure.MediatR.SqlServer` and/or `rbkApiModules.Infrastructure.MediatR.MongoDB`
  * `rbkApiModules.Utilities`
* Analytics libraries, contains server side analytics (database usage and requests statistics):
  * `rbkApiModules.Infrastructure.Analytics.Core`
  * `rbkApiModules.Infrastructure.Analytics.SqlServer`
  * `rbkApiModules.Infrastructure.Analytics.UI`

* Audiring libraries, contains database auditing features (under development):
  * `rbkApiModules.Infrastructure.Auditing.Core`
  * `rbkApiModules.Infrastructure.Auditing.SqlServer`
  * `rbkApiModules.Infrastructure.Auditing.UI`

* Diagnostic libraries, contains API error loging and endpoints for client side error log:
  * `rbkApiModules.Infrastructure.Diagnostics.Commons`
  * `rbkApiModules.Infrastructure.Diagnostics.Core`
  * `rbkApiModules.Infrastructure.Diagnostics.SqlServer`
  * `rbkApiModules.Infrastructure.Diagnostics.UI`

* Authentication libraries, contains authentication features and roles/claims based authorization:
  * `rbkApiModules.Infrastructure.Authnetication`

* Centralized UI for all all other UI libraries:
  * `rbkApiModules.Infrastructure.SharedUI`

* UI Data Annotations for automatic dialogs/forms generation in frontend project using Angular and PrimeNG
  * `rbkApiModules.Infrastructure.UIAnnotations`

The suggested API architecture to be used with this library is the following:

![image](https://user-images.githubusercontent.com/10734059/92814257-4ae09700-f399-11ea-8a0d-c13da7e75119.png)

The `rbkApiModules.Infrastructure.Models` must be referenced in the `{MyProject}.Models` project.

The `rbkApiModules.Infrastructure.MediatR.Core` must be referenced in the `{MyProject}.BusinessLogic` project.

The `rbkApiModules.Infrastructure.MediatR.Core.SqlServer` and/or `rbkApiModules.Infrastructure.MediatR.Core.MongoDB` must be referenced in the `{MyProject}.BusinessLogic` project.

The `rbkApiModules.Infrastructure.Api` must be referenced in the `{MyProject}.Api` project.

The `rbkApiModules.Utilities.EFCore` must be referenced in the `{MyProject}.Database` project.

The `rbkApiModules.Utilities.MongoDB` must be referenced in the `{MyProject}.Database` and `{MyProject}.BusinessLogic` projects if the project uses MongoDB.

All other libraries are optional and need to be referenced only in the `{MyProject}.Api` project, with exception to the :

* `rbkApiModules.Authentication`, which also needs to be referenced in the `{MyProject}.Models` and `{MyProject}.Database` projects.

* `rbkApiModules.UIAnnotations`, which also needs to be referenced in the `{MyProject}.Models` projects.


# 2. rbkApiModules.Infrastructure.Api

This library needs to be configured in the `ConfigureServices` by calling the `AddRbkApiInfrastructureModule` like this:

```c#
var xmlPath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
services.AddRbkApiInfrastructureModule(AssembliesForServices, AssembliesForAutoMapper, "RbkApiModules Demo API", "v1", xmlPath, !Environment.IsDevelopment());

```
The parameters for this method are the following:

* `assembliesForServices`: array of `Assembly` objects containing services to be registered in the dependency injection container.
* `assembliesForAutoMapper`: array of `Assembly` objects containing `AutoMapper` configuration files to be registered.
* `applicationName`: application name (to be shown in the swagger page)
* `version`: API version (to be shown in the swagger page).
* `xmlPath`: disk path to the swagger xml file.
* `isProduction`: a boolean indicating whether the API is running in the `production` or `development` model.

By using this configuration method the following services will be setup in the API:

* HttpContextAccessor
* AutoMapper
* Swagger UI
* HttpClient
* MemoryCache
* Gzip Compression
* Https redirection
* Hsts

It also needs to be activated in the `Configure` method by using the `UseRbkApiDefaultSetup`.

```c#
app.UseRbkApiDefaultSetup(!Environment.IsDevelopment());
```

This library exposes an abstract `BaseController` which all controllers should inherit from. There are two important methods in this class, both used to return data to the client.

* `HttpResponse<T>`: this method should be used when the object returned from the `MediatR` command must be mapped to a DTO. If the command is successful the data in the `Result` property will be mapped to the `T` type.
```c#
[HttpPost]
public async Task<ActionResult<TreeNode[]>> Create(CreateCategory.Command data)
{
    return HttpResponse<TreeNode[]>(await Mediator.Send(data));
}

```

* `HttpResponse`: this method should be used when the object returned from the `MediatR` command doesn't need be mapped to a DTO. If the command is successful the data in the `Result` property will be mapped to the `T` type.
```c#
[HttpDelete("{id}")]
public async Task<ActionResult> Delete(Guid id)
{
    return HttpResponse(await Mediator.Send(new DeleteCategory.Command { Id = id }));
}

```

If an unhandled exception ocurred, the response to the client will be a code `500` and if a handled exception or a validation error ocurred, a code `400` will be returnded. In both cases the request body contains a list of errors in `string[]` format.


# 3. rbApiModules.Infrastructure.MediatR

This libray has all infrastructure needed for `MediatR` library. It must be configured in the `ConfigureServices` like this:

```c#
services.AddRbkApiMediatRModule(AssembliesForMediatR);
```

By using this setup method all assemblies passed through the `assembliesForMediatR` parameter will be scanned to find commands and handlers for `MediatR`. It will also setup the validation step in the `MediatR` pipeline. This library assumes that the API is using a SQL database.

The library also exposed the follwing for the user:

## 3.1. MediatR command handlers

For organization purposes the MediatR commands were split in two types: commands and queries. Each one has an abstract class that should be used in the actual implementations. The main objective of these classes is to provide a general `try/catch` for error handling. If an unhandled exception occurs in the execution of the command, the result in the action will be a code `500` and if a `SafeException` is thrown in the command execution the result in the action will be a code `400`.

### 3.2.1. BaseCommandHandler (for SQL Server)
The response will be of type `CommandResponse`. Following is an example of a command inheriting from this class:

```c#
public class CreateCategory
{
    public class Command : IRequest<CommandResponse>
    {
        ... my properties
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            _context = context;

            ... my validators
        }
    }

    public class Handler : BaseCommandHandler<Command, DatabaseContext>
    {
        public Handler(DatabaseContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
        }

        protected override async Task<(Guid? entityId, object result)> ExecuteAsync(Command request)
        {
            ... my business logic

            await _context.SaveChangesAsync();

            return (aggregateId, result);
        }
    }
}
```

The result that should be returned is a `Tupple<Guid?, object>`, in which the first parameter is the id of the agregate entity related to this operation, if it exists or is used. If not, you can pass a `null` value. The second parameter is the actual result that should be returned to the action. If the command doesn't return a value, you can pass a `null` value. The `aggregateId` is needed only if you are also using the `rbkApiModules.Auditing` library and can be safely ignored if you are not using it.

There are versions of `BaseCommandHandler` for MongoDB and for non database commands.

### 3.1.2. BaseQueryHandler

The response will be of type `QueryResponse`. Following is an example of a command inheriting from this class:

```c#
public class GetAllAccounts
{
    public class Command : IRequest<QueryResponse>
    {
        ... my properties
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            CascadeMode = CascadeMode.Stop;

            ... my validators
        }
    }

    public class Handler : BaseQueryHandler<Command, DatabaseContext>
    {
        public Handler(DatabaseContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
        }

        protected override async Task<object> ExecuteAsync(Command request)
        {
            ... my query logic

            return results;
        }
    }
}
```

The result that should be returned is an `object` containing the actual result that should be returned to the action.

The validation pipeline is also setup with this library. All command inputs should be validated in the validation step, this way when the command execution actually begins you can be sure that you are working with safe data. For instance, you can be sure that a given id exists in the database and if you need to query it, you can do it by using a `Single` statement instead of a `SingleOrDefault` followed by an `if` statement to check for `null`. This makes the command code very short and easy to read/maintain.

The `Validator` class will be the main resource of validations, but you can also use interface validator to validade common properties. For instance if you have many command using an `OrderId` property, you create and `IOrder` interface and a validator for this interface, then it will be automatically used in the validation step. The following snipped shows an example:"

There are versions of `BaseCommandHandler` for MongoDB and for non database commands.

```c#
public interface IOrder
{
    Guid OrderId { get; }
}

public class OrderValidation: AbstractValidator<IOrder>
{
    public OrderValidation()
    {
        RuleFor(x => x.OrderId)
            .MustExistInDatabase<IOrder, SellingOrder>();
    }
}

public class Command1: IOrder
{
    public Guid OrderId { get; set; }
}

public class Command2: IOrder
{
    public Guid OrderId { get; set; }
}
```

In the above example both `Command1` and `Command2` will have their `OrderId` properties validated.

The library also exposes the following validators:

* `MustExistInDatabase<T, T2>`: to check if the id exists in the database in the `T2` table. It can be used for `Guid` or `Guid?` properties. If used with the last, it will validade only when the property is not `null`, otherwise it will always return `true`.

* `IsRequired`

* `MustNotBeNull`

* `MustHasLengthBetween`

* `ListMustHaveItems`

* `MustBePhone`

* `MustBeEmail`

# 4. rbkApiModules.Infrastructure.Models

This library contains the following helping classes:

* `BaseDataTransferObject`: class containing a `string` id to be inherited by DTOs
* `SimpleNamedEntity` and `SimpleNamedEntity<T>`: utility class helpful to exchange data with the client application, containing an `id` and `name` properties.
* `BaseEntity`: base class to be used for entities that will be stored in the database with a `Guid` id.
* `SafeException`: when this exception is thrown in a command handler, it will be treated as `400` return code to the cliente application.
* `IPassword`: this interface should be implemented in all commands containing a password property if the `rbkApiModules.Auditing` is used. This will ensure that the password will be anonymized when saved in the auditing database.

# 5. rbkApiModules.Utilities

Most of the contents of this library is meant be used by the other libraries in this repository, even though they have a few usefull extension methods:

* `ApplyConfigurations`: extension for the `ModelBuilder` class to automatically scan assemblies for `EF Core` contiguration files.
* `GetUsername`: extensions for the `IHttpContextAccessor` to get the username of the authenticated user from the JWT token.
* `ToUnixEpochDate`: extension for `DateTime` objects to convert them to Unix epoch dates (in seconds)
* `EmailHandler.SendMail`: Method for sending email via SMTP containing attachments and images in the body of the email, which can be in text, HTML, or both formats.


# 6. rbkApiModules.Analytics

The analytics libraries are a set of libraries containing all infrastructure needed for server side analytics logging with SQL Server, MongoDB (under development) or SQLite (under development) data stores.

## 6.1 Installation

Add the `rbkApiModules.Analytics.{SqlServer/MongoDB/SQLite}` to the API project. This will store all analytics data in the database. If you want to expose the UI with all the charts and query features, also add the `rbkApiModules.Analytics.UI` library.

## 6.2 How to use

In your `Startup.cs` add the following code to your `ConfigureServices` method, after the call of `AddRbkApiInfrastructureModule`:

```c#
    services.AddSqlServerRbkApiAnalyticsModule(Configuration.GetConnectionString("AnalyticsConnection");
```

In your `Startup.cs` add the following code to you `Configure` method:

```c#
    app.UseSqlServerRbkApiAnalyticsModule();
```

You can pass an options object to the above method to configure what will be logged, for instance the following code will only log statistics of `/api` endpoints and exclude all `OPTIONS` requests.

```c#
    app.UseSqlServerRbkApiAnalyticsModule(options => options
        .LimitToPath("/api")
        .ExcludeMethods("OPTIONS")
    );
```

The following methods can be chained in the options object:

* `.Exclude(IPAddress ip)`
* `.LimitToPath(string path)`
* `.ExcludePath(params string[] paths)`
* `.ExcludeExtensions(params string[] extensions)`
* `.ExcludeMethods(params string[] methods)`
* `.ExcludeLoopback()`
* `.ExcludeIp(IPAddress address)`
* `.ExcludeStatusCodes(params HttpStatusCode[] codes)`
* `.ExcludeStatusCodes(params int[] codes)`
* `.LimitToStatusCodes(params HttpStatusCode[] codes)`
* `.LimitToStatusCodes(params int[] codes)`

## 6.2 Features

Besides all analytics logging you will have access to an `/api/analytics` route with many endpoints to retrieve the data. If you are using the UI library you will also have access to the `/analytics/filter` and `/analytics/dashboard` routes.

The filter route allows for querying all the analytics data in an easy way:

![image](https://user-images.githubusercontent.com/22370391/98444471-98a32280-20f0-11eb-8fc9-5b27cff87fff.png)

The dashboard route provides a set os charts about the API usage:

![image](https://user-images.githubusercontent.com/22370391/98444544-06e7e500-20f1-11eb-8cfa-b9d179e716f5.png)

The following data is provided in the dashboard UI:

* Daily active users
* Daily errors
* Top users
* Most failed endpoints
* Endpoints error rates
* Daily authentication failures
* Daily inbound traffic
* Daily outbound traffic
* Biggest requests
* Beggest responses
* Daily requests
* Mostu used endpoints
* Slowest endpoints
* Total processing time
* Total database time
* Daily database transactions
* Average transactions per endpoint
* Application most active hours
* Application most active week days

For every request, the following information is stored:

* Version: application version (not implemented yet)
* Area: application area, gathered form the `ApplicationArea` attribute, that can decorate Actions or Controllers
* Timestamp: UTC time of the request
* Identity: request identity (from ASP.NET Core)
* Username: username from the JWT token if the request is authenticated
* Domain: from the JWT token in a `domain` claim
* IpAddress: client application IP address
* UserAgent: client application User Agent
* Method: HTTP method
* Path: requested URL
* Action: action path that handled the request
* Response size: size of the response body
* Request size: size of the request body
* Response: request response code
* Duration: total time for handling the request
* WasCached: if the request was retrieved from `MemoryCache` or database
* Total database time: total time spent with database processing
* Transaction count: total travels made to the database

# 7. rbkApiModules.Auditing

Under development.

# 8. rbkApiModules.Authentication

The authentication library contains all infrastructure needed for authentication and authorization in the application. These features are availabe for SQL Server databases as for the time being.

## 8.1 Installation

Add the `rbkApiModules.Authentication` library to you API and Database projects.

## 8.2 How to use

In your `Startup.cs` file, in the `ConfigureServices` method, add the following code after the call to `AddRbkApiInfrastructureModule`:

```c#
services.AddRbkApiAuthenticationModule(Configuration.GetSection(nameof(JwtIssuerOptions)));
```

In your `DatabaseContext`, add and entry in the `ApplyConfigurations` method to scan the authentication entities in the library:

```c#
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurations(new []
        {
            typeof(DatabaseContext).Assembly,
            typeof(UserLogin.Command).Assembly, // <-- Add this line
        });
    }
```

Add a new migration to the project and all authentication classes will be mapped by EF Core. The classes in the library are the following:

![image](https://user-images.githubusercontent.com/22370391/98446209-ae1d4a00-20fa-11eb-8709-8a676af3e095.png)

Basically there is an abstract `BaseUser` that must be implemented in your application. This user can have multiple roles. A role represents a group of claims, and a claim is the smallest authorization unit in your application. You can use them to control access to your controllers/actions or control access to actions in your frontend. Besides that, you can also override claims directly to a user (either allowing it or denying it).

The library will expose the following endpoints:

![image](https://user-images.githubusercontent.com/22370391/98447172-6d74ff00-2101-11eb-95d5-23510308ae3f.png)
![image](https://user-images.githubusercontent.com/22370391/98447173-7534a380-2101-11eb-8cfc-653d5cb867a8.png)
![image](https://user-images.githubusercontent.com/22370391/98447176-7d8cde80-2101-11eb-97ba-5d6842b5b148.png)
![image](https://user-images.githubusercontent.com/22370391/98447183-87aedd00-2101-11eb-9bb9-58cf82455884.png)
![image](https://user-images.githubusercontent.com/22370391/98447190-94333580-2101-11eb-99ab-813111f713b2.png)

The library [ngx-rbk-utils](https://github.com/rbasniak/ngx-rbk-utils) has some utility features to use this roles/claims architecture to handle authorization in the frontend.

# 9. rbkApiModules.Comments

Under development.

# 10. rbkApiModules.UIAnnotations

The UI Annotation library is meant to be used with both [ngx-rbk-utils](https://github.com/rbasniak/ngx-rbk-utils) and [ngx-smz-dialogs](https://github.com/smarza/ngx-smz). The objective of this library is to provide an endpoint with all information needed to create the forms for the creation and update of the entities in the Angular frontend.

This is very useful in heavy CRUD applications, because with only 2 lines of code you have the entire UI for the form and the backend is always the source of truth for the models.

I had one common issue in all companies and projects I have worked in the past: it's very hard to keep the UI synchronized with the backend model, specially the validations like required, minimun length or maximum length. To solve this problem we came up with a solution that evolved to this library.

Following is an example of a property using Data Annotations and in the frontend will be translated to: a required input radio, visible only in the update dialog of the entity, labeled 'Active' in a group called 'Extra Data'.

```c#
    [Required]
    [DialogData(OperationType.Update, "Active", Group = "Extra Data", ForcedType = DialogControlTypes.Radio)]
    public bool IsActive { get; private set; }
```

For the `required`, `minimum lenght` and `maximum length` validations, the native .NET data annotations are used, because EF Core will also use them to create the database schema. All other options must be set in the `DialogData` attribute.

Only two properties are required in the attribute: the operation type in which the input should appear in the frontend and the label of the input.

The `OperationType` uses the following enum:

```c#
    public enum OperationType
    {
        Create,
        Update,
        CreateAndUpdate
    }
```

This is enough for basic forms, everything else is infered by conventions, but can also be individually overriden. The conventions used are the following:

* properties of type `string` with maximum length less than 100 are shown as input texts
* properties of type `string` with maximum length greater than 100 are shown as text areas
* properties of type `bool` are shown as checkboxes
* properties of type `int`, `long`, `single` and `double` are shown as numeric inputs
* properties of type `DateTime` are shown as calendar inputs
* properties of type `Enum` are shown as dropdowns and the items are automatically get from the enum values as a `SimpleNamedEntity[]`. If the enum value has a `Description` attribute, it will be used instead of the value name.
* properties of type `Guid` are shown as input texts
* properties of type `List<Type>` will be shown as multiselect dropdowns
* properties of type inherithing from `BaseEntity` are shown as dropdowns or child forms depending on the `Source` property

The following properties can be optionally set in the `DialogData` attribute:

* `FordedType`: can be used to override the default input types, like changing an input text by a text area input. The available types are the ones present in the `DialogControlTypes` enum.
* `Source`: source for properties inheriting from `BaseEntity`:
  * `ChildForm`: instead of the input for the property, a child form will be used instead
  * `Store`: the dropdown items comes from a NGXS database state in the frontend
* `DefaultValue`: default value for the input
* `Group`: name of the group for the inputs
* `IsVisible`: visibility of the input
* `ExcludeFromResponse`: flag indicating if the value of the property should be excluded from the form response
* `DependsOn`: used for linked dropdowns and must be set with the name of the linked input
* `TextAreaRows`: number of rows for text area inputs
* `Mask`: PrimeNG mask pattern for masked inputs
* `Unmask`: PrimeNG unmask pattern for masked inputs
* `CharacterPattern`: PrimeNG character patterns for masked inputs
* `FileAccept`: types of accepted files for file input
* `ShowFilter`: flag indicating if the dropdown has a filter for the items
* `FilterMatchMode`: filter match mode for PrimeNG filtered dropdowns
* `SourceName`: name of the state to be used as item source for dropdowns
* `EntityLabelPropertyName`: by default the `Name` property of an entity is used to create the dropdown items. If the class doesn't have a `Name` property you need to specify which property will be used for the display name of the items
* `OverridePropertyName`: by default the name of the properties are the name of the property but starting in lower case, if you need to override the property name it can be done by this property






















--------------------------------------------------------------------

# Incluir no readme

4 Chamar o ApplyConfigurantions no DatabaseContext

services.AddTransient<DbContext, DatabaseContext>();


"Microsoft.EntityFrameworkCore.Database.Command": "Information"

            services.AddDbContext<DatabaseContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection").Replace("**CONTEXT**", "Database"))
                .EnableSensitiveDataLogging());


                    public class UserConfig: BaseUserConfig, IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> entity)
        {
            base.Configure<User>(entity, 512, 1024);

            entity.HasOne(x => x.Client)
                .WithOne(x => x.User)
                .HasForeignKey<User>(x => x.Id);
        }
    }


# TODO DOC

.API
  HttpResponse<T> e HttpResponse, quando eles retornam valores e quando não, como funciona o cache. Quando retorna 400 e 500
  AntiXXS middleware


MediatR
  Base Command Handler x2 mostrar o try/catch
  BaseQueryHandler x2
  Validators
  Validadores de interface

Models
  explicar cada classe

Utilities
  tudo

# TODO
- Localizar todas as mensagens
- Transformar cards do radzor em componentes de grafico e tabela, cada um com seu request
- Transformar tableas biggest responses/requests em string/string com pipe no size
- Escolher a unidade
- Criar readio nos graficos de tamanho para escolher a unidade mb, kg, gb
- Na tabela de most used read endpoint tem um endpoint em branco com muitos acessos
- Unificar most used endpoints
- Adicionar graficos de
  - Daily Average Requests Per Hour'
  - Geolocation data
  - Most active days (grafico de coluna) como tratar timezone?
  - Most active hours (grafico de coluna) como tratar timezone?
  - Criar conceito de sessão de uso (automatica? colocar no fonrt por interceptor?)
- Pegar a versão da aplicação de algum lugar
- Adicionar campos extras no filtro de analytics
- Melhorar a UI geral
- Ver se é possivel colocar o filtro ApplicationArea no controller
- Pensar em autenticacao no front geral (custom authorize com user/password?)
- Criar validador com interface para todos os dates de filtro. Tem que estar em utc
- Fazer tratamento de erros nas paginas
- Fazer spinner nas paginas






