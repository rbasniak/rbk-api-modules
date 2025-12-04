using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace rbkApiModules.Analysers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class EndpointAuthorizationAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "RBK201";

        private const string Category = "Security";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            "Missing AllowAnonymous or RequireAuthorization on endpoint",
            "Endpoint for '{0} {1}' should declare .AllowAnonymous() or .RequireAuthorization() to specify the authorization policy",
            Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Ensure that the endpoint declares AllowAnonymous() or RequireAuthorization() to specify the authorization policy.");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
        }

        private void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            // Only analyze methods that might contain endpoints
            if (methodDeclaration.Body == null)
            {
                return;
            }

            // Look for MapXXX calls in the method body
            var mapHttpCalls = methodDeclaration.Body.DescendantNodes()
                .OfType<InvocationExpressionSyntax>()
                .Where(invocation => FindMapHttpCall(invocation) != null)
                .ToList();

            foreach (var mapHttpCall in mapHttpCalls)
            {
                var actualMapHttpCall = FindMapHttpCall(mapHttpCall);
                if (actualMapHttpCall == null)
                {
                    continue;
                }

                // Check if there's a .Produces<T>() call in the same method
                var producesFound = methodDeclaration.Body.DescendantNodes()
                    .OfType<InvocationExpressionSyntax>()
                    .Where(inv =>
                    {
                        if (inv.Expression is MemberAccessExpressionSyntax macc &&
                            (macc.Name.Identifier.Text == "AllowAnonymous" || macc.Name.Identifier.Text == "RequireAuthorization"))
                        {
                            return true;
                        }
                        return false;
                    })
                    .Any();

                if (!producesFound)
                {
                    // Get the route template from the MapXXX call
                    var routeArg = actualMapHttpCall.ArgumentList.Arguments.FirstOrDefault()?.Expression;
                    string routeTemplate = "unknown";

                    if (routeArg is LiteralExpressionSyntax literal)
                    {
                        routeTemplate = literal.Token.ValueText;
                    }

                    var httpMethod = ((MemberAccessExpressionSyntax)actualMapHttpCall.Expression).Name.Identifier.Text.Replace("Map", "").ToUpper();

                    var diag = Diagnostic.Create(Rule,
                        actualMapHttpCall.Expression.GetLocation(),
                        httpMethod,
                        routeTemplate);
                    context.ReportDiagnostic(diag);
                }
            }
        }

        private InvocationExpressionSyntax FindMapHttpCall(InvocationExpressionSyntax invocation)
        {
            // System.Diagnostics.Debugger.Launch();

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

                // Move up the chain
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