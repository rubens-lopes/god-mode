using GodMode.Domain.Extensions;

namespace GodMode.Domain.Entities;

using SentenceDictionary = Dictionary<MetaData, Sentence>;

public record Crossword
{
  private readonly SentenceDictionary _across = new();
  private readonly SentenceDictionary _down = new();
  private readonly List<Cell> _cells = new();

  public Sentence? LongestSentenceAcross => _across.Any()
    ? _across.MaxBy(pair => pair.Value.Length).Value
    : null;

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
      if (cell.Coordinate.IsPartialSentence(cells)) AddToSentence(cell, cells);

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

  private void AddToSentence(Cell cell, Cell[] cells)
  {
    if (cell.Coordinate.HasStartCell(cells))
      _across
        .First(pair => pair.Key.LocalOrdinal == cell.Coordinate.Row)
        .Value
        .Add(cell.Character);

    if (cell.Coordinate.HasTopCell(cells) == false) return;

    _down.First(pair => pair.Key.LocalOrdinal == cell.Coordinate.Column)
      .Value
      .Add(cell.Character);
  }

  public IEnumerable<Sentence> Sentences => _across.Values
    .Concat(_down.Values)
    .ToArray();

  public void Solve(string[] possibleAnswers)
  {
    foreach (var sentence in Sentences)
      sentence.TryToSolve(possibleAnswers);
  }
}