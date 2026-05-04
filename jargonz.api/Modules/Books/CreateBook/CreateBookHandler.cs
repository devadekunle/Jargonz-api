using jargonz.api.Common.Persistence;
using jargonz.api.Common.Results;
using jargonz.api.Domain;

namespace jargonz.api.Modules.Books.CreateBook;

public static class CreateBookHandler
{
    private static readonly string[] CoverColors =
    [
        "#5B4A3A", "#2E4057", "#6B4E71", "#3A7D5C", "#8B4513",
        "#4A6FA5", "#C0392B", "#1A5276", "#7D6608", "#117A65",
        "#6C3483", "#1F618D", "#935116", "#0E6655", "#922B21"
    ];

    public static async Task<Result<CreateBookResponse>> HandleAsync(
        CreateBookCommand command,
        DataContext context,
        CancellationToken ct)
    {
        var random = Random.Shared.Next(CoverColors.Length);
        var coverColor = CoverColors[random];

        var book = new Book
        {
            Id = Ulid.NewUlid(),
            Title = command.Title,
            Author = command.Author,
            CoverColor = coverColor,
            UserId = command.UserId,
            CreatedAt = DateTime.UtcNow
        };

        context.Books.Add(book);
        await context.SaveChangesAsync(ct);

        var response = new CreateBookResponse(
            book.Id,
            book.Title,
            book.Author,
            book.CoverColor,
            book.CreatedAt
        );

        return Result.Success(response);
    }
}
