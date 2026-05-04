using FluentValidation;

namespace jargonz.api.Modules.Books.CreateBook;

public record CreateBookCommand(string Title, string Author, Ulid UserId);

public record CreateBookResponse(
    Ulid Id,
    string Title,
    string Author,
    string CoverColor,
    DateTime CreatedAt);

public class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
{
    public CreateBookCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(500).WithMessage("Title must not exceed 500 characters");

        RuleFor(x => x.Author)
            .NotEmpty().WithMessage("Author is required")
            .MaximumLength(300).WithMessage("Author must not exceed 300 characters");
    }
}
