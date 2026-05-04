using System.IO.Hashing;
using System.Text;
using Microsoft.EntityFrameworkCore;
using jargonz.api.Common.Persistence;
using jargonz.api.Common.Results;
using jargonz.api.Domain;

namespace jargonz.api.Modules.Words.WordOfTheDay;

public static class WordOfTheDayHandler
{
    public static async Task<Result<WordOfTheDayResponse>> HandleAsync(
        WordOfTheDayQuery query,
        DataContext context,
        CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        // Step 1: Check if we already have a cached WOTD for this user + today
        var cachedWotd = await context.WordOfTheDayCaches
            .Where(c => c.UserId == query.UserId && c.Date == today)
            .Select(c => new WordOfTheDayResponse(
                c.WordEntry.Id,
                c.WordEntry.Word,
                c.WordEntry.Definition,
                c.WordEntry.Phonetic,
                c.WordEntry.PartOfSpeech,
                c.WordEntry.ExampleSentence,
                c.WordEntry.BookId,
                c.WordEntry.Book.Title
            ))
            .FirstOrDefaultAsync(ct);

        if (cachedWotd is not null)
            return Result.Success(cachedWotd);

        // Step 2: No cache found — compute the WOTD deterministically
        var wordCount = await context.WordEntries
            .CountAsync(w => w.UserId == query.UserId, ct);

        if (wordCount == 0)
            return Error.NotFound("WordEntry", query.UserId.ToString());

        var index = DeriveIndex(query, today, wordCount);

        var word = await context.WordEntries
            .Include(w => w.Book)
            .Where(w => w.UserId == query.UserId)
            .OrderBy(w => w.Id)
            .Skip(index)
            .Take(1)
            .FirstOrDefaultAsync(ct);

        if (word is null)
            return Error.NotFound("WordEntry", query.UserId.ToString());

        // Step 3: Persist the cache so the WOTD stays stable for the rest of the day
        var cacheEntry = new WordOfTheDayCache
        {
            UserId = query.UserId,
            Date = today,
            WordEntryId = word.Id,
            CachedAt = DateTime.UtcNow
        };

        context.WordOfTheDayCaches.Add(cacheEntry);
        await context.SaveChangesAsync(ct);

        return Result.Success(MapToResponse(word));
    }

    private static int DeriveIndex(WordOfTheDayQuery query, DateOnly today, int wordCount)
    {
        // Compute deterministic index using XxHash32 (stable across restarts)
        var key = $"{query.UserId}|{today:yyyy-MM-dd}";
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var hash = XxHash32.HashToUInt32(keyBytes);
        var index = (int)(hash % (uint)wordCount);
        return index;
    }

    private static WordOfTheDayResponse MapToResponse(WordEntry word)
    {
        return new WordOfTheDayResponse(
            word.Id,
            word.Word,
            word.Definition,
            word.Phonetic,
            word.PartOfSpeech,
            word.ExampleSentence,
            word.BookId,
            word.Book.Title
        );
    }
}
