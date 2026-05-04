namespace jargonz.api.Modules.Words.WordOfTheDay;

public record WordOfTheDayResponse(
    Ulid Id,
    string Word,
    string Definition,
    string Phonetic,
    string PartOfSpeech,
    string ExampleSentence,
    Ulid BookId,
    string BookTitle);
