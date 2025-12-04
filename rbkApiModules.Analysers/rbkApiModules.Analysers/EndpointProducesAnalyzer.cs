using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace rbkApiModules.Analysers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class EndpointProducesAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId_MissingProduces = "RBK101";
        public const string DiagnosticId_WrongProducesType = "RBK102";
        public const string DiagnosticId_MissingProducesVoid = "RBK103";
        public const string DiagnosticId_ProducesVoidWithReturnType = "RBK104";
        public const string DiagnosticId_HandlerReturnsMultipleTypes = "RBK105";

        private static readonly string Category = "Swagger";

        private static readonly DiagnosticDescriptor MissingProducesRule = new DiagnosticDescriptor(
            DiagnosticId_MissingProduces,
            "Missing Produces<T> on endpoint",
            "Endpoint should declare .Produces<T>() to specify the return type",
            Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor WrongProducesTypeRule = new DiagnosticDescriptor(
            DiagnosticId_WrongProducesType,
            "Produces<T> type does not match",
            "Produces<T> type '{0}' does not match actual return type '{1}'",
            Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor MissingProducesVoidRule = new DiagnosticDescriptor(
            DiagnosticId_MissingProducesVoid,
            "Missing Produces() for void endpoint",
            "Endpoint should declare .Produces() to specify no content is returned",
            Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor ProducesVoidWithReturnTypeRule = new DiagnosticDescriptor(
            DiagnosticId_ProducesVoidWithReturnType,
            "Produces() used, but endpoint returns a value",
            "Endpoint declares .Produces() (void) but returns a value of type '{0}'",
            Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor HandlerReturnsMultipleTypesRule = new DiagnosticDescriptor(
            DiagnosticId_HandlerReturnsMultipleTypes,
            "Handler returns multiple types",
            "Handler is returning {0} which differs from other return calls",
            Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(
                MissingProducesRule,
                WrongProducesTypeRule,
                MissingProducesVoidRule,
                ProducesVoidWithReturnTypeRule,
                HandlerReturnsMultipleTypesRule
            );

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
        }

        private void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;

            // Check if this is a MapPost, MapPut, MapGet or MapDelete call (either direct or in a chain)
            var mapHttpCall = FindMapHttpCall(invocation);
            if (mapHttpCall == null)
                return;

            // Try to extract the handler (second argument of MapXXX)
            var handlerArgument = mapHttpCall.ArgumentList.Arguments.ElementAtOrDefault(1)?.Expression;
            if (handlerArgument is not AnonymousFunctionExpressionSyntax lambda)
                return;

            // Look for SendAsync invocation inside the handler
            var sendAsyncInvocation = lambda.DescendantNodes()
                .OfType<InvocationExpressionSyntax>()
                .FirstOrDefault(inv =>
                    inv.Expression is MemberAccessExpressionSyntax macc &&
                    macc.Name.Identifier.Text == "SendAsync"
                );
            if (sendAsyncInvocation == null)
                return;

            // Get the type of the first argument to SendAsync
            var firstArg = sendAsyncInvocation.ArgumentList.Arguments.FirstOrDefault()?.Expression;
            if (firstArg == null)
                return;

            var typeInfo = context.SemanticModel.GetTypeInfo(firstArg);
            var requestType = typeInfo.Type;
            if (requestType == null)
                return;

            // --- Hardcoded interface checks ---
            var isQuery = requestType.AllInterfaces.Any(i => i.ToDisplayString() == "IQuery" || i.ToDisplayString().EndsWith(".IQuery"));
            var isCommand = requestType.AllInterfaces.Any(i => i.ToDisplayString() == "ICommand" || i.ToDisplayString().EndsWith(".ICommand"));
            if (!(isQuery || isCommand))
                return;

            // 1. Is the type named "Request"?
            var isRequestNamed = requestType.Name == "Request";

            // 2. Is it a nested class?
            var parentType = requestType.ContainingType;
            var isNested = parentType != null;

            INamedTypeSymbol handlerType = null;
            var hasHandler = false;
            if (isRequestNamed && isNested && parentType != null)
            {
                handlerType = parentType
                    .GetTypeMembers()
                    .FirstOrDefault(x => x.TypeKind == TypeKind.Class && x.Name == "Handler");
                hasHandler = handlerType != null;
            }

            if (!isRequestNamed || !hasHandler || handlerType == null)
                return;

            // Find the HandleAsync method in Handler
            var handleAsync = handlerType.GetMembers()
                .OfType<IMethodSymbol>()
                .FirstOrDefault(x => x.Name == "HandleAsync" && x.MethodKind == MethodKind.Ordinary);

            ITypeSymbol returnType = null;
            if (handleAsync != null && handleAsync.DeclaringSyntaxReferences.Length > 0)
            {
                var handleAsyncSyntax = handleAsync.DeclaringSyntaxReferences[0].GetSyntax() as MethodDeclarationSyntax;
                if (handleAsyncSyntax != null)
                {
                    // Find all invocations to CommandResponse.Success or QueryResponse.Success
                    var successCalls = handleAsyncSyntax.DescendantNodes()
                        .OfType<InvocationExpressionSyntax>()
                        .Where(inv =>
                        {
                            if (inv.Expression is MemberAccessExpressionSyntax macc)
                            {
                                var methodName = macc.Name.Identifier.Text;
                                if (methodName == "Success")
                                {
                                    var exprSymbol = context.SemanticModel.GetSymbolInfo(macc.Expression).Symbol;
                                    var exprSymbolDisplay = exprSymbol?.ToDisplayString();
                                    return exprSymbolDisplay == "CommandResponse" || exprSymbolDisplay == "QueryResponse"
                                        || (exprSymbolDisplay?.EndsWith(".CommandResponse") == true)
                                        || (exprSymbolDisplay?.EndsWith(".QueryResponse") == true);
                                }
                            }
                            return false;
                        })
                        .ToList();

                    var argTypes = new List<ITypeSymbol>();
                    foreach (var call in successCalls)
                    {
                        var args = call.ArgumentList.Arguments;
                        if (args.Count == 0)
                            continue; // parameterless means 'void'/'null'
                        var arg = args[0].Expression;
                        var argType = context.SemanticModel.GetTypeInfo(arg).Type;
                        if (argType != null)
                            argTypes.Add(argType);
                    }

                    if (argTypes.Count == 0)
                    {
                        returnType = null; // void (all calls parameterless)
                    }
                    else if (argTypes.Distinct(SymbolEqualityComparer.Default).Count() == 1)
                    {
                        returnType = argTypes[0];
                    }
                    else
                    {
                        foreach (var successCall in successCalls)
                        {
                            var args = successCall.ArgumentList.Arguments;
                            if (args.Count == 0)
                            {
                                var returnTypeMethod = isQuery ? "QueryResponse.Success()" : "CommandResponse.Success()";

                                var diag = Diagnostic.Create(HandlerReturnsMultipleTypesRule, successCall.GetLocation(), returnTypeMethod);
                                context.ReportDiagnostic(diag);
                            }
                            else
                            {
                                var arg = args[0].Expression;
                                var argType = context.SemanticModel.GetTypeInfo(arg).Type;
                                if (argType != null)
                                {
                                    var returnTypeMethod = isQuery ? $"QueryResponse.Success({argType?.ToDisplayString()})" : $"CommandResponse.Success({argType?.ToDisplayString()})";

                                    var diag = Diagnostic.Create(HandlerReturnsMultipleTypesRule, successCall.GetLocation(), returnTypeMethod);
                                    context.ReportDiagnostic(diag);
                                }
                            }
                        }

                        returnType = argTypes.Last();
                    }
                }
            }

            // -------- Find Produces calls in the method --------
            var parentMethod = invocation.Ancestors().OfType<MethodDeclarationSyntax>().FirstOrDefault();
            if (parentMethod?.Body == null)
                return;

            var producesCalls = parentMethod.Body.DescendantNodes()
                .OfType<InvocationExpressionSyntax>()
                .Where(inv =>
                {
                    if (inv.Expression is MemberAccessExpressionSyntax macc &&
                        macc.Name.Identifier.Text == "Produces")
                    {
                        return true;
                    }
                    return false;
                })
                .ToList();

            var producesGeneric = producesCalls
                .Where(inv => inv.Expression is MemberAccessExpressionSyntax macc && macc.Name is GenericNameSyntax)
                .ToList();

            var producesVoid = producesCalls
                .Where(inv => !(inv.Expression is MemberAccessExpressionSyntax macc && macc.Name is GenericNameSyntax))
                .ToList();

            // -------- Compare and Report --------
            if (returnType == null)
            {
                // Should use Produces() (void)
                if (producesVoid.Count == 0)
                {
                    // Missing Produces()
                    var diag = Diagnostic.Create(MissingProducesVoidRule, mapHttpCall.Expression.GetLocation());
                    context.ReportDiagnostic(diag);
                }
                else if (producesGeneric.Count > 0)
                {
                    // Produces<T> declared but actual return type is void
                    var producesT = producesGeneric.First();
                    var tArg = ((GenericNameSyntax)((MemberAccessExpressionSyntax)producesT.Expression).Name).TypeArgumentList.Arguments.First();
                    var tType = context.SemanticModel.GetTypeInfo(tArg).Type;
                    var diag = Diagnostic.Create(ProducesVoidWithReturnTypeRule, producesT.GetLocation(), tType?.ToDisplayString() ?? "unknown");
                    context.ReportDiagnostic(diag);
                }
            }
            else
            {
                // Should use Produces<T> matching returnType
                if (producesGeneric.Count == 0)
                {
                    // Missing Produces<T>
                    var diag = Diagnostic.Create(MissingProducesRule, mapHttpCall.Expression.GetLocation());
                    context.ReportDiagnostic(diag);
                }
                else
                {
                    // Check the <T>
                    var producesT = producesGeneric.First();
                    var tArg = ((GenericNameSyntax)((MemberAccessExpressionSyntax)producesT.Expression).Name).TypeArgumentList.Arguments.First();
                    var tType = context.SemanticModel.GetTypeInfo(tArg).Type;

                    if (!SymbolEqualityComparer.Default.Equals(tType, returnType))
                    {
                        var diag = Diagnostic.Create(WrongProducesTypeRule, producesT.GetLocation(), tType?.ToDisplayString() ?? "unknown", returnType?.ToDisplayString() ?? "unknown");
                        context.ReportDiagnostic(diag);
                    }
                }

                // Produces() used together with non-void? Warn as well
                if (producesVoid.Count > 0)
                {
                    var producesVoidCall = producesVoid.First();
                    var diag = Diagnostic.Create(ProducesVoidWithReturnTypeRule, producesVoidCall.GetLocation(), returnType?.ToDisplayString() ?? "unknown");
                    context.ReportDiagnostic(diag);
                }
            }
        }

        private InvocationExpressionSyntax FindMapHttpCall(InvocationExpressionSyntax invocation)
        {
            var supportedMethods = new[] { "MapPost", "MapGet", "MapPut", "MapDelete" };

            if (invocation.Expression is MemberAccessExpressionSyntax memberAccess &&
                supportedMethods.Contains(memberAccess.Name.Identifier.Text))
            {
                return invocation;
            }

            // Check if this is part of a method chain that starts with MapXXX
            var current = invocation;
            while (current != null)
            {
                if (current.Expression is MemberAccessExpressionSyntax macc &&
                    supportedMethods.Contains(macc.Name.Identifier.Text))
                {
                    return current;
                }

                if (current.Parent is InvocationExpressionSyntax parentInvocation)
                {
                    current = parentInvocation;
                }
                else
                {
                    break;
                }
            }

            return null;
        }
    }
}
