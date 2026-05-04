namespace jargonz.api.Modules.Books.GetBooks;

public record GetBooksQuery(Ulid UserId);

public record GetBooksResponse(
    Ulid Id,
    string Title,
    string Author,
    string CoverColor,
    DateTime CreatedAt);
