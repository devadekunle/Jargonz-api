using jargonz.api.Common.Extensions;
using jargonz.api.Modules.Words.CheckDuplicateWord;
using jargonz.api.Modules.Words.CreateWord;
using jargonz.api.Modules.Words.GetWords;
using jargonz.api.Modules.Words.WordOfTheDay;

namespace jargonz.api.Modules.Words;

/// <summary>
///     Words module - groups all word-related endpoints
/// </summary>
public class WordsModule : IModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1")
            .WithTags("Words");

        group.MapCreateWordEndpoint();
        group.MapGetWordsEndpoint();
        group.MapCheckDuplicateWordEndpoint();
        group.MapWordOfTheDayEndpoint();
    }
}
