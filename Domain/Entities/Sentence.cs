using System.Text.RegularExpressions;
using static System.String;

namespace GodMode.Domain.Entities;

public record Sentence
{
  private List<Character> Characters { get; } = new();
  public int Length => Characters.Count;

  public bool IsSolved => Characters
    .All(character => character.HasValue);

  public Sentence(params Character[] characters) => Add(characters);

  public void Add(params Character[] characters)
  {
    Characters.AddRange(characters);
  }

  public override string ToString()
  {
    var sentence = Characters
      .Select((character, index) => index == 0
        ? character.ToString().ToUpper()
        : character.ToString().ToLower()
      )
      .ToArray();

    return Join("", sentence);
  }

  public bool TryToSolve(IEnumerable<string> possibleAnswers)
  {
    var regexTokens = Characters
      .Select(character => character.ToRegexToken())
      .ToArray();

    var regex = new Regex($"^{Join("", regexTokens)}$", RegexOptions.IgnoreCase);

    var answers = possibleAnswers
      .Where(possibleAnswer => regex.IsMatch(possibleAnswer))
      .Distinct()
      .ToArray();

    if (answers.Length is 0 or > 1) return false;

    var answer = answers.First();
    var indexedCharacters = Characters
      .Select((character, index) => new {character, index});

    foreach (var pair in indexedCharacters)
      pair.character.Set(answer[pair.index]);

    return true;
  }
}