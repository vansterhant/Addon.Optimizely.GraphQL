using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Addon.Optimizely.GraphQL.Generator;

[Generator]
public class GraphQLIncrementalSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider
            .CreateSyntaxProvider(
                (s, _) => s is ClassDeclarationSyntax,
                (ctx, _) => GetClassDeclarationForSourceGen(ctx))
            .Where(t => t.shouldGenerate)
            .Select((t, _) => t.syntax);
        
        
        // Generate the source code.
        context.RegisterSourceOutput(context.CompilationProvider.Combine(provider.Collect()),
            (ctx, t) => GenerateCode(ctx, t.Left, t.Right));
    }
    
    private static (List<INamedTypeSymbol> syntax, bool shouldGenerate) GetClassDeclarationForSourceGen(
        GeneratorSyntaxContext context)
    {
        var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;

        if (context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax) is not INamedTypeSymbol namedSymbol)
        {
            return (Enumerable.Empty<INamedTypeSymbol>().ToList(), false);
        }
        
        var (isContentData, symbols) = namedSymbol.FindTypeHierarchy("EPiServer.Core.ContentData");

        if (isContentData is false)
        { 
            return (Enumerable.Empty<INamedTypeSymbol>().ToList(), false);
        }
        
        // Go through all attributes of the class.
        foreach (var attributeSyntax in classDeclarationSyntax.AttributeLists.SelectMany(attributeListSyntax => attributeListSyntax.Attributes))
        {
            if (context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol
                || !attributeSymbol.ContainingType.IsDerivedFromType("EPiServer.DataAnnotations.ContentTypeAttribute"))
            {
                continue;
            }

            return (symbols, symbols.Count > 0);
        }

        return (Enumerable.Empty<INamedTypeSymbol>().ToList(), false);
    }
    
    private void GenerateCode(SourceProductionContext context, Compilation compilation,
        ImmutableArray<List<INamedTypeSymbol>> classDeclarations)
    {
        var types = new List<ClassMetadata>();
        
        // Go through all filtered class declarations.
        foreach (var classSymbols in classDeclarations)
        {
            var baseClass = GetBaseClass(classSymbols);

            if (baseClass is null)
            {
                continue;
            }
            
            var classSymbol = classSymbols.First();
            var classMetadata = new ClassMetadata
                { 
                    Symbol = classSymbol,
                    NamespaceName = classSymbol.ContainingNamespace.ToDisplayString(), 
                    ClassName = classSymbol.Name, 
                    BaseClass = baseClass, 
                    Fields = CodeGenerators.GetFields(classSymbols)
                };
            
            types.Add(classMetadata);
            
            // Add the source code to the compilation.
            context.AddSource(
                $"{classMetadata.ClassName}Type.g.cs", 
                SourceText.From(CodeGenerators.GetTypeClass(classMetadata, baseClass != "BlockData"), Encoding.UTF8));
            
            if (baseClass == "BlockData")
            {
                context.AddSource($"{classMetadata.ClassName}Type_shared.g.cs", SourceText.From(CodeGenerators.GetSharedBlockFacade(classMetadata), Encoding.UTF8));
            }
        }
        
        context.AddSource($"PageChildrenType.g.cs", SourceText.From(CodeGenerators.GetPageChildrenType(types), Encoding.UTF8));
        context.AddSource($"ContentAreaItemFactory.g.cs", SourceText.From(CodeGenerators.GetContentAreaItemsFactory(types), Encoding.UTF8));
        context.AddSource($"SchemaRequestExecutorBuilderExtensions.g.cs", SourceText.From(CodeGenerators.GetSchemaBuilderExtension(types), Encoding.UTF8));
        context.AddSource($"Query.g.cs", SourceText.From(CodeGenerators.GetQuery(types), Encoding.UTF8));
    }

    private static string? GetBaseClass(List<INamedTypeSymbol> classSymbols)
    {
        if (classSymbols.Exists(c => c.Name == "PageData"))
        {
            return "PageData";
        }
        
        if (classSymbols.Exists(c => c.Name == "BlockData"))
        {
            return "BlockData";
        }

        if (classSymbols.Exists(c => c.Name == "MediaData"))
        {
            return "MediaData";
        }

        return null;
    }
}

public struct Field
{
    public IPropertySymbol Symbol { get; set; }
    public string? Type { get; set; }

    public string? Descriptor =>
        Type switch
        {
            null => null,
            "ContentReferenceType" => $"descriptor.Field(t => t.{Symbol.Name}).Type<StringType>().Resolve(context => context.Parent<PageData>().{Symbol.Name}.ToString());",
            "ListType<ContentReferenceType>" => $"descriptor.Field(t => t.{Symbol.Name}).Type<StringType>().Resolve(context => context.Parent<PageData>().{Symbol.Name}.Select(x => x.ToString()));",
            _ => $"descriptor.Field(t => t.{Symbol.Name}).Type<{Type}>();"
        };

    public string? FilterType { get; set; }
    public string? SortType { get; set; }
}

public struct ClassMetadata
{
    public INamedTypeSymbol Symbol { get; set; }
    public string NamespaceName { get; set; }
    public string ClassName { get; set; }
    public string BaseClass { get; set; }
    
    public bool IsBlock => BaseClass == "BlockData";
    
    public string FilterName => (IsBlock ? "Shared" : "") + $"{ClassName}FilterType";
    public string SortName => (IsBlock ? "Shared" : "") + $"{ClassName}SortType";

    public List<Field> Fields { get; set; }
}