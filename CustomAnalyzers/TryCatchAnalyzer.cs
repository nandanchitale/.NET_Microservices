using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class TryCatchAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "TryCatchAnalyzer";

    private static readonly LocalizableString Title = "Function must have try-catch block";
    private static readonly LocalizableString MessageFormat = "The function '{0}' must have a try-catch block.";
    private static readonly LocalizableString Description = "Every function should have a try-catch block.";
    private const string Category = "ErrorHandling";

    private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.MethodDeclaration);
    }

    private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
    {
        var methodDeclaration = (MethodDeclarationSyntax)context.Node;

        var hasTryCatch = methodDeclaration.Body?.DescendantNodes().OfType<TryStatementSyntax>().Any() ?? false;

        if (hasTryCatch)
            return;

        var diagnostic = Diagnostic.Create(Rule, methodDeclaration.Identifier.GetLocation(), methodDeclaration.Identifier.Text);
        context.ReportDiagnostic(diagnostic);
    }
}
