using EPiServer.DataAbstraction;
using HotChocolate.Types;

namespace Addon.Optimizely.GraphQL;

public class LanguageType : EnumType<string>
{
    private readonly IList<LanguageBranch> _languageBranches;

    public LanguageType(IList<LanguageBranch> languageBranches)
    {
        _languageBranches = languageBranches;
    }

    protected override void Configure(IEnumTypeDescriptor<string> descriptor)
    {
        descriptor.Name("Language");

        foreach (var languageBranch in _languageBranches)
            descriptor
                .Value(languageBranch.LanguageID)
                .Name(languageBranch.LanguageID);
    }
}