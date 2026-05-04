# Word of the Day Endpoint — Implementation Plan (v2 - Scalable)

## Overview

Implement `GET /api/v1/words/wotd` — a per-user, deterministic Word of the Day endpoint. Returns a simplified response for the same word all day, selected deterministically using a seed based on `userId + today's date`.

## Route

```
GET /api/v1/words/wotd
Authorization: Bearer <token>
```

## Response (200 OK)

```json
{
  "id": "01J...",
  "word": "serendipity",
  "definition": "The occurrence of events by chance in a happy or beneficial way.",
  "phonetic": "/ˌsɛrənˈdɪpɪti/",
  "partOfSpeech": "noun",
  "exampleSentence": "It was pure serendipity that they met at the conference."
}
```

## Response (404 Not Found)

Returned when the user has no words saved yet.

## Scalable Algorithm

The key insight: we don't need to load all words into memory. We can do this in **two efficient SQL queries**:

1. **`COUNT` query**: Get the total number of words for the user — `SELECT COUNT(*) FROM WordEntries WHERE UserId = @userId`
2. **`SKIP/TAKE` query**: Use the deterministic index to fetch just the one word — `SELECT ... FROM WordEntries WHERE UserId = @userId ORDER BY Id OFFSET @index LIMIT 1`

This means:
- **Memory**: O(1) — only one word row is ever loaded
- **Database**: Two lightweight queries (COUNT + OFFSET FETCH) — both use the index on `UserId`
- **Scales**: Works identically for 10 words or 100,000 words

### Deterministic Index Calculation

```csharp
var today = DateOnly.FromDateTime(DateTime.UtcNow);
var seed = HashCode.Combine(userId, today);
var index = Math.Abs(seed) % wordCount;
```

This ensures:
- Same user + same date → same word
- Same user + different date → different word (unless only 1 word exists)
- Different users + same date → different words (different userId in seed)

## Files to Create/Modify

| File | Action | Description |
|------|--------|-------------|
| [`WordOfTheDayResponse.cs`](jargonz.api/Modules/Words/WordOfTheDay/WordOfTheDayResponse.cs) | **Modify** | Simplify to Id, Word, Definition, Phonetic, PartOfSpeech, ExampleSentence |
| [`WordOfTheDayEndpoint.cs`](jargonz.api/Modules/Words/WordOfTheDay/WordOfTheDayEndpoint.cs) | **Create** | Maps `GET /words/wotd`, extracts userId, calls handler |
| [`WordOfTheDayHandler.cs`](jargonz.api/Modules/Words/WordOfTheDay/WordOfTheDayHandler.cs) | **Create** | COUNT query → compute index → OFFSET FETCH one row |
| [`WordsModule.cs`](jargonz.api/Modules/Words/WordsModule.cs) | **Modify** | Register `MapWordOfTheDayEndpoint()` |

## Detailed File Contents

### 1. `WordOfTheDayResponse.cs` (Modify)

```csharp
namespace jargonz.api.Modules.Words.WordOfTheDay;

public record WordOfTheDayResponse(
    Ulid Id,
    string Word,
    string Definition,
    string Phonetic,
    string PartOfSpeech,
    string ExampleSentence
);
```

### 2. `WordOfTheDayEndpoint.cs` (Create)

Follows the existing Wolverine pattern — uses `IMessageBus.InvokeAsync()` to dispatch to the handler, same as [`GetWordsEndpoint`](../jargonz.api/Modules/Words/GetWords/GetWordsEndpoint.cs) and [`CreateWordEndpoint`](../jargonz.api/Modules/Words/CreateWord/CreateWordEndpoint.cs).

```csharp
using jargonz.api.Common.Extensions;
using jargonz.api.Common.Results;
using Wolverine;

namespace jargonz.api.Modules.Words.WordOfTheDay;

public static class WordOfTheDayEndpoint
{
    public static IEndpointRouteBuilder MapWordOfTheDayEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/words/wotd", async (IMessageBus bus, HttpContext httpContext) =>
            {
                var userId = httpContext.GetUserId();
                var query = new WordOfTheDayQuery(userId);
                var result = await bus.InvokeAsync<Result<WordOfTheDayResponse>>(query);
                return result.ToHttpResult();
            })
            .RequireAuthorization()
            .WithName("WordOfTheDay")
            .WithTags("Words")
            .WithSummary("Get the Word of the Day")
            .WithDescription("Returns a deterministic word of the day for the authenticated user. The same word is shown all day, and changes daily.")
            .Produces<WordOfTheDayResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}
```

### 3. `WordOfTheDayHandler.cs` (Create) — Scalable version

```csharp
using Microsoft.EntityFrameworkCore;
using jargonz.api.Common.Persistence;

namespace jargonz.api.Modules.Words.WordOfTheDay;

public static class WordOfTheDayHandler
{
    public static async Task<WordOfTheDayResponse?> HandleAsync(
        Ulid userId,
        DataContext context,
        CancellationToken ct = default)
    {
        // Step 1: Efficient COUNT query — only hits the index, no row data loaded
        var wordCount = await context.WordEntries
            .CountAsync(w => w.UserId == userId, ct);

        if (wordCount == 0)
            return null;

        // Step 2: Compute deterministic index
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var seed = HashCode.Combine(userId, today);
        var index = Math.Abs(seed) % wordCount;

        // Step 3: Single-row OFFSET FETCH — only loads one word into memory
        var word = await context.WordEntries
            .Where(w => w.UserId == userId)
            .OrderBy(w => w.Id)
            .Skip(index)
            .Take(1)
            .Select(w => new WordOfTheDayResponse(
                w.Id,
                w.Word,
                w.Definition,
                w.Phonetic,
                w.PartOfSpeech,
                w.ExampleSentence
            ))
            .FirstOrDefaultAsync(ct);

        return word;
    }
}
```

### 4. `WordsModule.cs` (Modify)

Add the using and endpoint registration:

```csharp
using jargonz.api.Modules.Words.WordOfTheDay;

// In MapEndpoints:
group.MapWordOfTheDayEndpoint();
```

## Flow Diagram

```mermaid
flowchart TD
    A[Client: GET /api/v1/words/wotd] --> B[WordOfTheDayEndpoint]
    B --> C[Extract userId from JWT claims]
    C --> D[WordOfTheDayHandler.HandleAsync]
    D --> E[COUNT WordEntries WHERE UserId = userId]
    E --> F{wordCount == 0?}
    F -->|Yes| G[Return 404 Not Found]
    F -->|No| H[Compute seed = HashCode.Combine userId + today's date]
    H --> I[index = Abs(seed) % wordCount]
    I --> J[SELECT 1 row: OFFSET index LIMIT 1]
    J --> K[Return 200 OK with WordOfTheDayResponse]
```

## Why This Scales

| Approach | Memory | DB Queries | DB Rows Loaded | Scales to |
|----------|--------|------------|----------------|-----------|
| **v1 (load all)** | O(n) | 1 | All user words | ~1,000 words |
| **v2 (COUNT + OFFSET)** | O(1) | 2 | 1 row | Unlimited |

The `UserId` column is indexed (via the FK relationship in [`WordEntryConfig`](jargonz.api/Common/Persistence/EntityConfigurations/WordEntryConfig.cs)), so both COUNT and OFFSET queries are efficient index operations.

## Dependencies

- [`HttpContextExtensions.GetUserId()`](jargonz.api/Common/Extensions/HttpContextExtensions.cs) — extracts Ulid from JWT claims
- [`DataContext`](jargonz.api/Common/Persistence/DataContext.cs) — EF Core DbContext with `WordEntries` DbSet
- [`WordEntry`](jargonz.api/Domain/WordEntry.cs) — domain entity with `UserId`, `Word`, `Definition`, `Phonetic`, `PartOfSpeech`

## Stability Analysis: What Happens When Words Change Mid-Day?

Since we use `OFFSET index` with `ORDER BY Id`, the stability depends on the type of change:

| Action | Effect on WOTD | Why |
|--------|---------------|-----|
| **Add a word** | ✅ **No change** | New words append at the end (Ulid is time-sorted). Existing words keep their relative positions. |
| **Delete a word before the WOTD index** | ⚠️ **Shifts to next word** | All subsequent words shift left by one position. |
| **Delete a word after the WOTD index** | ✅ **No change** | Words before the index are unaffected. |
| **Delete the WOTD itself** | ⚠️ **Shifts to next word** | The word that was at `index + 1` now occupies position `index`. |

**Is this a problem?** For a Word of the Day feature, this is acceptable behavior:
- Adding words mid-day is the most common operation and is **stable** — the WOTD won't change.
- Deleting the WOTD is rare, and showing the next word is a reasonable fallback.
- If strict immutability is desired, we could cache the WOTD per user per day (e.g., in Redis or a `WordOfTheDay` table), but that adds complexity beyond what's needed for this feature.

## Testing Considerations

- **No words**: Verify 404 response when user has no entries
- **Determinism**: Same user on same date should get same word
- **Daily change**: Same user on different dates should get different words (unless only 1 word exists)
- **Multiple users**: Different users on same date should get different words
- **Add word mid-day**: Verify WOTD doesn't change after adding a new word
- **Delete WOTD mid-day**: Verify the next word becomes the WOTD
- **Large datasets**: Verify performance with thousands of words per user
