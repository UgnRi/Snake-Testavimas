using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

/*
 Instructions for updating package:
change .csproj version
build on release
dotnet build -c Release
dotnet pack -c Release
dotnet nuget push C:\Users\marti\Desktop\GItHub\Snake-Testavimas\PerformanceAnalyzer\PerformanceAnalyzer.Package\bin\Release\PerformanceAnalyzer.x.x.0.nupkg -k oy2fajj3ecxlytirpqpqixkhbh5ilj5ujddyg2pwgmbyo4 -s https://api.nuget.org/v3/index.json

 
 
 
 */

namespace PerformanceAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class PerformanceAnalyzerAnalyzer : DiagnosticAnalyzer
    {
        public const string ClassNamingDiagnosticId = "PerformanceAnalyzer_ClassNaming";
        public const string UncachedCountDiagnosticId = "PerformanceAnalyzer_UncachedCount";
        public const string ExcessiveParametersDiagnosticId = "PerformanceAnalyzer_ExcessiveParameters";
        public const string MissingXmlDocDiagnosticId = "PerformanceAnalyzer_MissingXmlDoc";

        public static readonly DiagnosticDescriptor ClassNamingRule = new DiagnosticDescriptor(
            ClassNamingDiagnosticId,
            "Class naming issue",
            "Class '{0}' should be renamed to follow naming conventions.",
            "Naming",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor UncachedCountRule = new DiagnosticDescriptor(
            UncachedCountDiagnosticId,
            "Uncached collection.Count in loop",
            "Consider caching collection.Count outside the loop for better performance.",
            "Performance",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor ExcessiveParametersRule = new DiagnosticDescriptor(
            ExcessiveParametersDiagnosticId,
            "Method has too many parameters",
            "Method '{0}' has too many parameters. Consider reducing the number for better maintainability.",
            "Performance",
            DiagnosticSeverity.Info,
            isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor MissingXmlDocRule = new DiagnosticDescriptor(
            MissingXmlDocDiagnosticId,
            "Missing XML documentation",
            "Method '{0}' lacks XML documentation.",
            "Documentation",
            DiagnosticSeverity.Info,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
            ClassNamingRule,
            UncachedCountRule,
            ExcessiveParametersRule,
            MissingXmlDocRule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            // Register analysis actions
            context.RegisterSyntaxNodeAction(AnalyzeClassNaming, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeUncachedCount, SyntaxKind.ForStatement);
            context.RegisterSyntaxNodeAction(AnalyzeMethodParameters, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeMissingXmlDoc, SyntaxKind.MethodDeclaration);
        }

        private void AnalyzeClassNaming(SyntaxNodeAnalysisContext context)
        {
            var classDeclaration = (ClassDeclarationSyntax)context.Node;
            if (classDeclaration.Identifier.Text.Any(char.IsLower))
            {
                var diagnostic = Diagnostic.Create(ClassNamingRule, classDeclaration.Identifier.GetLocation(), classDeclaration.Identifier.Text);
                context.ReportDiagnostic(diagnostic);
            }
        }

        private void AnalyzeUncachedCount(SyntaxNodeAnalysisContext context)
        {
            var forStatement = (ForStatementSyntax)context.Node;

            // Check if the loop condition contains collection.Count
            if (forStatement.Condition is BinaryExpressionSyntax condition &&
                condition.Right is MemberAccessExpressionSyntax memberAccess &&
                memberAccess.Name.Identifier.Text == "Count")
            {
                // Report a diagnostic if Count is used without caching
                var diagnostic = Diagnostic.Create(UncachedCountRule, memberAccess.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }

        private void AnalyzeMethodParameters(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            // Check for excessive parameters (e.g., more than 5)
            if (methodDeclaration.ParameterList.Parameters.Count > 5)
            {
                var diagnostic = Diagnostic.Create(
                    ExcessiveParametersRule,
                    methodDeclaration.Identifier.GetLocation(),
                    methodDeclaration.Identifier.Text);
                context.ReportDiagnostic(diagnostic);
            }
        }

        private void AnalyzeMissingXmlDoc(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            // Check for XML documentation above the method
            var trivia = methodDeclaration.GetLeadingTrivia();
            var hasXmlDoc = trivia.Any(t => t.Kind() == SyntaxKind.SingleLineDocumentationCommentTrivia);

            if (!hasXmlDoc)
            {
                var diagnostic = Diagnostic.Create(
                    MissingXmlDocRule,
                    methodDeclaration.Identifier.GetLocation(),
                    methodDeclaration.Identifier.Text);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
