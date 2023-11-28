using EPiServer.DataAbstraction;
using HotChocolate.Execution.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Addon.Optimizely.GraphQL;

public static class SchemaRequestExecutorBuilderExtensions
{
    public static IRequestExecutorBuilder AddCmsGraphQlCore(this IServiceCollection services)
    {
        services.AddSingleton<ContentQueryService>();
        
        return services.AddGraphQLServer()
            .InitializeOnStartup()
            .AddAuthorization()
            .AddFiltering()
            .AddSorting()
            .AddType<SiteDefinitionType>()
            .AddType<ContentInterfaceType>()
            .AddType<PageDataType>()
            .AddType<BlockType>()
            .AddType<PageDataInterfaceType>()
            .ConfigureSchema((provider, schemaBuilder) =>
            {
                schemaBuilder.AddType(
                    new LanguageType(provider.GetRequiredService<ILanguageBranchRepository>().ListEnabled()));
            });
    }
}

