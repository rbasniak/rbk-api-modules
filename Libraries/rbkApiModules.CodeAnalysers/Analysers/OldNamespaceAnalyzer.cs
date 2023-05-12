using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace rbkApiModules.Commons.CodeAnalysers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class OldNamespaceAnalyzer : DiagnosticAnalyzer
    {
        private const string Category = "Naming";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            "OldNamespaceUsage",
            "Using old style namespace (using brackets) is discouraged",
            "Using old style namespace (using brackets) is discouraged. Please use using statement instead.",
            Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.UsingDirective);
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var diagnostic = Diagnostic.Create(Rule, Location.None);
            context.ReportDiagnostic(diagnostic);
            //var usingDirective = (UsingDirectiveSyntax)context.Node;

            //if (usingDirective.StaticKeyword.IsKind(SyntaxKind.None) && usingDirective.Name.ToString().Contains("["))
            //{
            //    var diagnostic = Diagnostic.Create(Rule, usingDirective.GetLocation());
            //    context.ReportDiagnostic(diagnostic);
            //}
        }
    }
}
