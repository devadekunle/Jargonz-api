using Microsoft.AspNetCore.Mvc;
using jargonz.api.Common.Query;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace jargonz.api.Modules.Words.GetWords;

public record GetWordsQuery : QueryModel
{
    [BindNever] public Ulid? UserId { get; set; }
    [FromQuery(Name = "bookId")] public Ulid? BookId { get; set; }

    [FromQuery(Name = "mastery")] public string? Mastery { get; set; }

    [FromQuery(Name = "due")] public bool? Due { get; set; }
}

public record GetWordResponse(
    Ulid Id,
    string Word,
    Ulid BookId,
    string Definition,
    string Phonetic,
    string PartOfSpeech,
    string Etymology,
    string ContextSentence,
    string ExampleSentence,
    string UserNotes,
    int? PageNumber,
    int Repetitions,
    DateOnly NextReviewDate,
    int TimesReviewed,
    int TimesCorrect,
    DateTime CreatedAt);
