using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Addon.Optimizely.GraphQL.Generator;

public static class SymbolExtensions
{
    private static readonly SymbolDisplayFormat FullNameFormat = new (
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);
    
    public static bool HasAttribute(this ISymbol symbol, string attributeName)
    {
        return symbol.GetAttributes()
            .Any(x => x.AttributeClass?.ToDisplayString() == attributeName);
    }

    public static AttributeData? FindAttribute(this ISymbol symbol, string attributeName)
    {
        return symbol.GetAttributes()
            .FirstOrDefault(x => x.AttributeClass?.ToDisplayString() == attributeName);
    }

    public static bool IsDerivedFromType(this INamedTypeSymbol symbol, string typeName)
    {
        return FindTypeHierarchy(symbol, typeName).Item1;
    }
    
    public static (bool, List<INamedTypeSymbol>) FindTypeHierarchy(this INamedTypeSymbol symbol, string typeName)
    {
        var symbols = new List<INamedTypeSymbol>();
        
        while (true)
        {
            if (symbol.BaseType == null)
            {
                return (false, Enumerable.Empty<INamedTypeSymbol>().ToList());
            }
            
            if (symbol.ToDisplayString(FullNameFormat) == typeName)
            {
                return (true, symbols);
            }
            
            symbols.Add(symbol);
            symbol = symbol.BaseType;
        }
    }

    public static bool IsImplements(this INamedTypeSymbol symbol, string typeName)
    {
        return symbol.AllInterfaces.Any(x => x.ToDisplayString() == typeName);
    }
}