/*

FEATURES:
 - Central log point to push scope wide properties Link:LoggingBehavior.cs
 - Central performance log point Link:PerformanceBehaviour.cs
 - Central trace point for DB changes Link:AuditBehavior.cs
 - Central transaction manager Link:TransactionBehavior.cs
 - Individual entity audit trail Link:DatabaseContext.cs:private void UpdateAuditInfo()
 - Response objects are carried over from the MediatR layer to the endpoint and automatically handled based on the results Link:BlogsController.cs:return HttpResponse<AuthorDetails[]>(response);

REMARKS
 - Commands/validators/handlers are kept in the same file for simplicity, readability and development performance Link:PipelineValidationTest.cs
 - Controllers are kept very thin, acting just as a shortcut to MediatR commands in most cases: Link:MediatrController.cs
 - Simple database restraints are put directly in the Model files because they could be used for other purposes (i.e., automatic code generation),
   but some people think this couples your model layer to the database layer. Link:Blog.cs:[Required]
 - In EF Core DateTime objects are saved to the database without the Kind, so if you save an UTC DateTime property, when reading it back from the
   database, it will come with DateTimeKind = Unspecified, and the frontend doesn't convert the value to the current timezone. To solve that we 
   need a ValueConverter Link:DatabaseContext.cs:NullableDateTimeWithoutKindConverter

DEMOS:
 - Show how exceptions are logged Link:LogExceptionTest.cs
 - Validation layer and composed validations using interfaces Link:PipelineValidationTest.cs
 - Scoped log example Link:ScopeLogTest.cs

 
 
 
 TODO: retries/polly
       open telemetry
       multi request transaction with aspnet core
       api error codes (Morten will send some material)
       multitenancy
       lifecycle of entities
 
 */