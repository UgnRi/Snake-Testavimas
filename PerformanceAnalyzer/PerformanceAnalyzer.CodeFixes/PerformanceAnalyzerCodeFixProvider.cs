using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PerformanceAnalyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PerformanceAnalyzerCodeFixProvider))]
    public class PerformanceAnalyzerCodeFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds =>
            ImmutableArray.Create(
                PerformanceAnalyzerAnalyzer.ClassNamingDiagnosticId,
                PerformanceAnalyzerAnalyzer.UncachedCountDiagnosticId,
                PerformanceAnalyzerAnalyzer.ExcessiveParametersDiagnosticId,
                PerformanceAnalyzerAnalyzer.MissingXmlDocDiagnosticId
            );

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Handle each diagnostic type
            if (diagnostic.Id == PerformanceAnalyzerAnalyzer.ClassNamingDiagnosticId)
            {
                var classDeclaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().First();
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: "Rename class to follow naming convention",
                        createChangedDocument: c => RenameClassAsync(context.Document, classDeclaration, c),
                        equivalenceKey: "RenameClass"),
                    diagnostic);
            }
            else if (diagnostic.Id == PerformanceAnalyzerAnalyzer.UncachedCountDiagnosticId)
            {
                var memberAccess = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MemberAccessExpressionSyntax>().First();
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: "Cache Count value",
                        createChangedDocument: c => CacheCountAsync(context.Document, memberAccess, c),
                        equivalenceKey: "CacheCount"),
                    diagnostic);
            }
            else if (diagnostic.Id == PerformanceAnalyzerAnalyzer.ExcessiveParametersDiagnosticId)
            {
                var methodDeclaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: "Refactor excessive parameters",
                        createChangedDocument: c => RefactorExcessiveParametersAsync(context.Document, methodDeclaration, c),
                        equivalenceKey: "RefactorParameters"),
                    diagnostic);
            }
            else if (diagnostic.Id == PerformanceAnalyzerAnalyzer.MissingXmlDocDiagnosticId)
            {
                var methodDeclaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: "Add XML documentation",
                        createChangedDocument: c => AddXmlDocumentationAsync(context.Document, methodDeclaration, c),
                        equivalenceKey: "AddXmlDocumentation"),
                    diagnostic);
            }
        }

        // Helper for renaming classes
        private async Task<Document> RenameClassAsync(Document document, ClassDeclarationSyntax classDeclaration, CancellationToken cancellationToken)
        {
            var identifierToken = classDeclaration.Identifier;
            var newName = identifierToken.Text.ToUpperInvariant();

            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

            editor.ReplaceNode(classDeclaration, classDeclaration.WithIdentifier(SyntaxFactory.Identifier(newName)));

            return editor.GetChangedDocument();
        }

        // Helper for caching Count
        private async Task<Document> CacheCountAsync(Document document, MemberAccessExpressionSyntax memberAccess, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

            var variableName = "cachedCount";
            var variableDeclaration = SyntaxFactory.LocalDeclarationStatement(
                SyntaxFactory.VariableDeclaration(SyntaxFactory.IdentifierName("var"))
                    .WithVariables(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier(variableName))
                                .WithInitializer(SyntaxFactory.EqualsValueClause(memberAccess)))));

            var parentLoop = memberAccess.Ancestors().OfType<ForStatementSyntax>().First();
            editor.InsertBefore(parentLoop, variableDeclaration);

            var newMemberAccess = SyntaxFactory.IdentifierName(variableName);
            editor.ReplaceNode(memberAccess, newMemberAccess);

            return editor.GetChangedDocument();
        }

        // Helper for refactoring excessive parameters
        private async Task<Document> RefactorExcessiveParametersAsync(Document document, MethodDeclarationSyntax methodDeclaration, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

            // Example: Group parameters into a single class (very basic implementation)
            var newClassName = $"{methodDeclaration.Identifier.Text}Parameters";
            var parameterList = methodDeclaration.ParameterList.Parameters;

            // Create a new class for parameters
            var parameterClass = SyntaxFactory.ClassDeclaration(newClassName)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddMembers(parameterList.Select(p =>
                    SyntaxFactory.PropertyDeclaration(p.Type, p.Identifier)
                        .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))).ToArray());

            // Add the parameter class to the document
            editor.InsertBefore(methodDeclaration, parameterClass);

            // Update method to use the new parameter class
            var updatedMethod = methodDeclaration
                .WithParameterList(SyntaxFactory.ParameterList().AddParameters(
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("parameters"))
                        .WithType(SyntaxFactory.IdentifierName(newClassName))));

            editor.ReplaceNode(methodDeclaration, updatedMethod);

            return editor.GetChangedDocument();
        }

        // Helper for adding XML documentation
        private async Task<Document> AddXmlDocumentationAsync(Document document, MethodDeclarationSyntax methodDeclaration, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

            // Create the XML documentation structure
            var xmlTrivia = SyntaxFactory.TriviaList(
                SyntaxFactory.Trivia(
                    SyntaxFactory.DocumentationCommentTrivia(SyntaxKind.SingleLineDocumentationCommentTrivia)
                        .AddContent(
                            SyntaxFactory.XmlText("/// "),
                            SyntaxFactory.XmlElement(
                                SyntaxFactory.XmlElementStartTag(SyntaxFactory.XmlName("summary")),
                                SyntaxFactory.XmlElementEndTag(SyntaxFactory.XmlName("summary")))
                                .AddContent(
                                    SyntaxFactory.XmlText("Describe the purpose of this method.")
                                ),
                            SyntaxFactory.XmlText(Environment.NewLine)
                        )));

            // Add the XML documentation as leading trivia to the method
            var updatedMethod = methodDeclaration.WithLeadingTrivia(xmlTrivia);

            // Replace the old method with the updated method
            editor.ReplaceNode(methodDeclaration, updatedMethod);

            return editor.GetChangedDocument();
        }
    }
}
