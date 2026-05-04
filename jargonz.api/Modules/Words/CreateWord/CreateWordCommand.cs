using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace jargonz.api.Modules.Words.CreateWord;

public record CreateWordCommand(
    string Word,
    Ulid BookId,
    string? ContextSentence,
    int? PageNumber,
   [BindNever] Ulid UserId);

public record CreateWordResponse(
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
    double EaseFactor,
    int Interval,
    int Repetitions,
    DateOnly NextReviewDate,
    int TimesReviewed,
    int TimesCorrect,
    DateTime CreatedAt);

public class CreateWordCommandValidator : AbstractValidator<CreateWordCommand>
{
    public CreateWordCommandValidator()
    {
        RuleFor(x => x.Word)
            .NotEmpty().WithMessage("Word is required")
            .MaximumLength(300).WithMessage("Word must not exceed 300 characters");

        RuleFor(x => x.BookId)
            .NotEmpty().WithMessage("BookId is required");

        RuleFor(x => x.ContextSentence)
            .MaximumLength(2000).When(x => x.ContextSentence is not null)
            .WithMessage("Context sentence must not exceed 2000 characters");
    }
}
