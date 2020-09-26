[![Build Status](https://img.shields.io/appveyor/ci/thiagoloureiro/netcore-jwt-integrator-extension/master.svg)](https://ci.appveyor.com/project/thiagoloureiro/netcore-jwt-integrator-extension)

# 1. Introduction

These are a set of libraries to help scaffold production ready APIs in ASP.NET Core.

The suggested API architecture to be used with this library is the following:

![image](https://user-images.githubusercontent.com/10734059/92814257-4ae09700-f399-11ea-8a0d-c13da7e75119.png)

The `rbkApiModules.Infrastructure.Models` must be referenced in the `Database`, `BusinessLogic`, `Models`, and `DataTransfer` projects.

The `rbkApiModules.Infrastructure.MediatR` must be referenced in the `BusinessLogic` project.

The `rbkApiModules.Infrastructure.Api` must be referenced in the `Api` project.

The `rbkApiModules.Infrastructure.Utilities` must be referenced in the `Database` project.

All other libraries are optional and need to be referenced only in the `Api` project, with exception to the `rbkApiModules.Authentication`, which if is used needs to referenced in the `Models` project.


# 2. rbkApiModules.Infrastructure.Api

This library needs to be configured in the `ConfigureServices` by calling the `AddRbkApiInfrastructureModule` like this:

```c#
var xmlPath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
services.AddRbkApiInfrastructureModule(AssembliesForServices, AssembliesForAutoMapper, "RbkApiModules Demo API", "v1", xmlPath, !Environment.IsDevelopment());

```
The parameters for this method are the following:

* assembliesForServices: array of `Assembly` objects containing services to be registered in the dependency injection container.
* assembliesForAutoMapper: array of `Assembly` objects containing `AutoMapper` configuration files to be registered.
* applicationName: application name (to be shown in the swagger page)
* version: API version (to be shown in the swagger page).
* xmlPath: disk path to the swagger xml file.
* isProduction: a boolean indicating whether the API is running in the `production` or `development` model.

By using this configuration method the following services will be setup in the API:

* Cross Site Scripting Protection: based on the implementation in `https://www.loginradius.com/engineering/blog/anti-xss-middleware-asp-core/`.
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

In both cases if an unhandled exception ocurred, the response to the client will be a code `500` and if a handled exception or a validation error ocurred, a code `400` will be returnded. In both cases the request body contains a list of errors in `string[]` format.

# 3. rbApiModules.Infrastructure.MediatR

This libray has all infrastructure needed for `MediatR` library. It must be configured in the `ConfigureServices` like this:

```c#
services.AddRbkApiMediatRModule(AssembliesForMediatR);
```

By using this setup method all assemblies passed through the `assembliesForMediatR` parameter will be scanned to find commands and handlers for `MediatR`. It will also setup the validation step in the `MediatR` pipeline. This library assumes that the API is using a SQL database.

The library also exposed the follwing for the user:

## 3.1. MediatR command handlers

For organization purposes the MediatR commands were split in two types: commands and queries. Each one has an abstract class that should be used in the actual implementations. The main objective of these classes is to provide a general `try/catch` for error handling. If an unhandled exception occurs in the execution of the command, the result in the action will be a code `500` and if a `SafeException` is thrown in the command execution the result in the action will be a code `400`.

### 3.2.1. BaseCommandHandler
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
* `SimpleNamedEntity`: utility class helpful to exchange data with the client application, containing an `id` and `name` properties.
* `BaseEntity`: base class to be used for entities that will be stored in the database with a `Guid` id.
* `SafeException`: when this exception is thrown in a command handler, it will be treated as `400` return code to the cliente application.
* `IPassword`: this interface should be implemented in all commands containing a password property if the `rbkApiModules.Auditing` is used. This will ensure that the password will be anonymized when saved in the auditing database.

# 5. rbkApiModules.Infrastructure.Utilities

Most of the contents of this library is meant be used by the other libraries in this repository, even though they have a few usefull extension methods:

* `ApplyConfigurations`: extension for the `ModelBuilder` class to automatically scan assemblies for `EF Core` contiguration files.
* `GetUsername`: extensions for the `IHttpContextAccessor` to get the username of the authenticated user from the JWT token.
* `ToUnixEpochDate`: extension for `DateTime` objects to convert them to Unix epoch dates (in seconds)


# 6. rbkApiModules.Analytics

TODO

# 7. rbkApiModules.Auditing

TODO

# 8. rbkApiModules.Authentication

TODO


```c#
services.AddRbkApiAuthenticationModule(Configuration.GetSection(nameof(JwtIssuerOptions)));
```

# 9. rbkApiModules.Comments

TODO

# 10. rbkApiModules.UIAnnotations

TODO































--------------------------------------------------------------------

# Drafts

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






