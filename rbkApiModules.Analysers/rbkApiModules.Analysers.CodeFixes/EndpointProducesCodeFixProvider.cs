//using Microsoft.CodeAnalysis;
//using Microsoft.CodeAnalysis.CodeActions;
//using Microsoft.CodeAnalysis.CodeFixes;
//using Microsoft.CodeAnalysis.CSharp;
//using Microsoft.CodeAnalysis.CSharp.Syntax;
//using Microsoft.CodeAnalysis.Rename;
//using Microsoft.CodeAnalysis.Text;
//using System;
//using System.Collections.Generic;
//using System.Collections.Immutable;
//using System.Composition;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;

//namespace rbkApiModules.Analysers
//{
//    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EndpointProducesCodeFixProvider)), Shared]
//    public class EndpointProducesCodeFixProvider : CodeFixProvider
//    {
//        public override ImmutableArray<string> FixableDiagnosticIds =>
//            ImmutableArray.Create(EndpointProducesAnalyzer.DiagnosticId);

//        public override FixAllProvider GetFixAllProvider() =>
//            WellKnownFixAllProviders.BatchFixer;

//        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
//        {
//            var diagnostic = context.Diagnostics.First();
//            context.RegisterCodeFix(
//                CodeAction.Create(
//                    title: "Add Produces<T>()",
//                    createChangedDocument: c => AddProducesAsync(context.Document, diagnostic, c),
//                    equivalenceKey: "AddProduces"),
//                diagnostic);
//        }

//        private async Task<Document> AddProducesAsync(
//            Document document,
//            Diagnostic diagnostic,
//            CancellationToken cancellationToken)
//        {
//            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
//            var diagnosticSpan = diagnostic.Location.SourceSpan;
//            var node = root.FindNode(diagnosticSpan) as NameSyntax;
//            if (node == null)
//                return document;

//            // Find the full invocation chain (outermost InvocationExpression)
//            var invocation = node.AncestorsAndSelf().OfType<InvocationExpressionSyntax>().Last();

//            // Extract the actual type from properties
//            string actualTypeName;
//            if (!diagnostic.Properties.TryGetValue("ActualType", out actualTypeName) || actualTypeName == null)
//            {
//                actualTypeName = "object";
//            }

//            // Build .Produces<ActualType>() invocation
//            var producesGeneric = SyntaxFactory.GenericName(
//                    SyntaxFactory.Identifier("Produces"))
//                .WithTypeArgumentList(
//                    SyntaxFactory.TypeArgumentList(
//                        SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
//                            SyntaxFactory.ParseTypeName(actualTypeName))));

//            var producesAccess = SyntaxFactory.MemberAccessExpression(
//                SyntaxKind.SimpleMemberAccessExpression,
//                invocation,
//                producesGeneric);

//            var producesInvocation = SyntaxFactory.InvocationExpression(
//                producesAccess,
//                SyntaxFactory.ArgumentList());

//            // Insert the new invocation at end of chain
//            var newRoot = root.ReplaceNode(invocation, producesInvocation.WithTriviaFrom(invocation));
//            return document.WithSyntaxRoot(newRoot);
//        }
//    }
//}