using GodMode.Domain.Extensions;

namespace GodMode.Domain.Entities;

using SentenceDictionary = Dictionary<MetaData, Sentence>;

public record Crossword
{
  private readonly SentenceDictionary _across = new();
  private readonly SentenceDictionary _down = new();
  private readonly List<Cell> _cells = new();

  public Sentence LongestAcrossSentence => _across
    .MaxBy(pair => pair.Value.Length).Value;

  public Sentence LongestDownSentence => _down
    .MaxBy(pair => pair.Value.Length).Value;

  public IEnumerable<Cell> Cells => _cells.AsReadOnly();

  public Crossword(params Cell[] cells)
  {
    _cells.AddRange(cells);

    GatherSentences();
  }

  private void GatherSentences()
  {
    var ordinal = 1;
    var cells = _cells.ToArray();

    foreach (var cell in cells)
    {
      if (cell.Coordinate.IsPartialSentence(cells)) AddToSentence(cell, cells, ordinal);

      if (cell.Coordinate.IsStartOfSentence(cells) == false) continue;

      StartSentence(cell, ordinal, cells);

      ordinal++;
    }
  }

  private void StartSentence(Cell cell, int ordinal, Cell[] cells)
  {
    if (cell.Coordinate.IsStartingAcrossSentence(cells))
      _across.Add(new MetaData(ordinal, cell.Coordinate.Row), new Sentence(cell.Character));

    if (cell.Coordinate.IsStartingDownSentence(cells))
      _down.Add(new MetaData(ordinal, cell.Coordinate.Column), new Sentence(cell.Character));
  }

  private void AddToSentence(Cell cell, Cell[] cells, int ordinal)
  {
    if (cell.Coordinate.HasStartCell(cells))
      _across
        .OrderByDescending(pair => pair.Key.Ordinal)
        .First(pair => pair.Key.LocalOrdinal == cell.Coordinate.Row && pair.Key.Ordinal <= ordinal)
        .Value
        .Add(cell.Character);

    if (cell.Coordinate.HasTopCell(cells) == false) return;

    _down
      .OrderByDescending(pair => pair.Key.Ordinal)
      .First(pair => pair.Key.LocalOrdinal == cell.Coordinate.Column && pair.Key.Ordinal <= ordinal)
      .Value
      .Add(cell.Character);
  }

  public IEnumerable<Sentence> Sentences => _across.Values
    .Concat(_down.Values)
    .ToArray();

  public IEnumerable<SolvedOrNotSentence> AcrossSentences => SentencesAsSolvedOrNotStrings(_across);
  public IEnumerable<SolvedOrNotSentence> DownSentences => SentencesAsSolvedOrNotStrings(_down);

  public void Solve(string[] possibleAnswers)
  {
    int solved;

    do
    {
      solved = Sentences.Count(sentence => sentence.IsSolved);

      foreach (var sentence in Sentences.Where(sentence => sentence.IsSolved == false))
        sentence.TryToSolve(possibleAnswers);
    } while (solved != Sentences.Count(sentence => sentence.IsSolved));
  }

  private static IEnumerable<SolvedOrNotSentence> SentencesAsSolvedOrNotStrings(SentenceDictionary dictionary) =>
    dictionary
      .Select(pair => new SolvedOrNotSentence(pair.Key.Ordinal, pair.Value))
      .ToArray();
}