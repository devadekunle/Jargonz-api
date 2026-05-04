using Microsoft.EntityFrameworkCore;
using jargonz.api.Common.Extensions;
using jargonz.api.Common.Llm;
using jargonz.api.Common.Persistence;
using jargonz.api.Common.Results;
using jargonz.api.Domain;

namespace jargonz.api.Modules.Words.CreateWord;

public class CreateWordHandler(ILogger<CreateWordHandler> logger)
{
    public async Task<Result<CreateWordResponse>> HandleAsync(
        CreateWordCommand command,
        DataContext context,
        WordEnrichmentService enrichmentService,
        ILogger<CreateWordHandler> logger,
        CancellationToken ct)
    {
        var book = await context.Books
            .FirstOrDefaultAsync(b => b.Id == command.BookId, ct);

        if (book is null)
            return Error.NotFound("Book", command.BookId.ToString());

        var wordHash = command.Word.ToWordHash(command.UserId, command.ContextSentence! );

        var duplicateExists = await context.WordEntries
            .AnyAsync(w => w.WordHash == wordHash, ct);

        if (duplicateExists)
            return Error.Conflict(
                "WordAlreadyExists",
                $"The word '{command.Word}' already exists in your vocabulary.");

        var wordEntry = new WordEntry
        {
            Id = Ulid.NewUlid(),
            Word = command.Word,
            WordHash = wordHash,
            BookId = command.BookId,
            ContextSentence = command.ContextSentence ?? string.Empty,
            PageNumber = command.PageNumber,
            UserId = command.UserId,
            CreatedAt = DateTime.UtcNow
        };

        // Enrich the word entry with LLM data (definition, phonetic, etc.)
        var enrichment = await enrichmentService.EnrichWordAsync(
            command.Word, command.ContextSentence, ct);

        if (enrichment is not null)
        {
            wordEntry.Definition = enrichment.Definition;
            wordEntry.Phonetic = enrichment.Phonetic;
            wordEntry.PartOfSpeech = enrichment.PartOfSpeech;
            wordEntry.Etymology = enrichment.Etymology;
            wordEntry.ExampleSentence = enrichment.ExampleSentence;

            logger.LogInformation("Word '{Word}' enriched successfully by LLM", command.Word);
        }
        else
        {
            logger.LogWarning(
                "LLM enrichment failed for word '{Word}'. Word will be saved without enrichment data.",
                command.Word);
        }

        context.WordEntries.Add(wordEntry);
        await context.SaveChangesAsync(ct);

        var response = new CreateWordResponse(
            wordEntry.Id,
            wordEntry.Word,
            wordEntry.BookId,
            wordEntry.Definition,
            wordEntry.Phonetic,
            wordEntry.PartOfSpeech,
            wordEntry.Etymology,
            wordEntry.ContextSentence,
            wordEntry.ExampleSentence,
            wordEntry.UserNotes,
            wordEntry.PageNumber,
            wordEntry.EaseFactor,
            wordEntry.Interval,
            wordEntry.Repetitions,
            wordEntry.NextReviewDate,
            wordEntry.TimesReviewed,
            wordEntry.TimesCorrect,
            wordEntry.CreatedAt
        );

        return Result.Success(response);
    }
}
