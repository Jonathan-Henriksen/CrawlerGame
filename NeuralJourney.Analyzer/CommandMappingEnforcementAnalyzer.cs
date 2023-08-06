namespace NeuralJourney.Analyzer
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Collections.Immutable;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CommandMappingEnforcementAnalyzer : DiagnosticAnalyzer
    {
        private const string Category = "Usage";
        private const string Title = "Missing CommandMappingAttribute";
        private const string MessageFormat = "Type '{0}' must be decorated with CommandMappingAttribute";
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor("MY001", Title, MessageFormat, Category, DiagnosticSeverity.Warning, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
        }

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            var namedTypeSymbol = (INamedTypeSymbol) context.Symbol;

            if (namedTypeSymbol.BaseType?.ToString() == "Command")
            {
                if (!namedTypeSymbol.GetAttributes().Any(attr => attr.AttributeClass?.ToString() == "CommandMappingAttribute"))
                {
                    var diagnostic = Diagnostic.Create(Rule, namedTypeSymbol.Locations[0], namedTypeSymbol.Name);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}