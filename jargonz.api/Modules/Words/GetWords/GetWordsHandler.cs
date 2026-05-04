using Microsoft.EntityFrameworkCore;
using jargonz.api.Common.Persistence;
using jargonz.api.Common.Query;
using jargonz.api.Common.Results;

namespace jargonz.api.Modules.Words.GetWords;

public static class GetWordsHandler
{
    public static async Task<Result<PaginatedResponse<GetWordResponse>>> HandleAsync(
        GetWordsQuery query,
        DataContext context,
        CancellationToken ct)
    {
        var wordsQuery = context.WordEntries
            .Where(w => w.UserId == query.UserId)
            .AsQueryable();

        // Filter by book
        if (query.BookId.HasValue)
            wordsQuery = wordsQuery.Where(w => w.BookId == query.BookId.Value);

      

        // Filter by due for review
        if (query.Due.HasValue && query.Due.Value)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            wordsQuery = wordsQuery.Where(w => w.NextReviewDate <= today);
        }

        // Search by word or definition
        if (!string.IsNullOrWhiteSpace(query.SearchKeyword))
        {
            var keyword = query.SearchKeyword.ToLower();
            wordsQuery = wordsQuery.Where(w =>
                w.Word.ToLower().Contains(keyword) ||
                w.Definition.ToLower().Contains(keyword));
        }

        return await wordsQuery
            .OrderByDescending(w => w.Id)
            .Select(w => new GetWordResponse(
                w.Id,
                w.Word,
                w.BookId,
                w.Definition,
                w.Phonetic,
                w.PartOfSpeech,
                w.Etymology,
                w.ContextSentence,
                w.ExampleSentence,
                w.UserNotes,
                w.PageNumber,
                w.Repetitions,
                w.NextReviewDate,
                w.TimesReviewed,
                w.TimesCorrect,
                w.CreatedAt
            ))
            .ApplyPaginationAsync(query, ct);
    }
}
