using GodMode.Domain.Entities;

namespace GodMode.Cli;

public static class Drawer
{
  private const int CellWidth = 3;

  public static void DrawCrossword(Crossword crossword)
  {
    var width = crossword.LongestAcrossSentence.Length * CellWidth;
    var height = crossword.LongestDownSentence.Length;

    for (var line = 0; line < height; line++)
      Console.WriteLine();

    var origin = GetOrigin(width, height);

    foreach (var cell in crossword.Cells.ToArray())
      DrawCell(cell, origin);

    Console.WriteLine();

    DrawCrosswordSummary(crossword.AcrossSentences, "Across: ", origin, width);
    DrawCrosswordSummary(crossword.DownSentences, "Down: ", origin, width);
  }

  private static void DrawCrosswordSummary(IEnumerable<SolvedOrNotSentence> sentences,
    string label,
    Coordinate origin,
    int width,
    string separator = ". ")
  {
    var lineLength = label.Length;
    Console.SetCursorPosition(origin.Row, Console.CursorTop);

    Util.WriteInMagenta(label);

    var acrossSentences = sentences.ToArray();
    var last = acrossSentences.Last();

    foreach (var sentence in acrossSentences)
    {
      var sentenceString = sentence.ToString();
      var length = sentenceString.Length;
      var shouldBreakLine = lineLength + length >= width;

      if (shouldBreakLine)
      {
        Console.WriteLine();
        Console.SetCursorPosition(origin.Row, Console.CursorTop);
      }

      lineLength = shouldBreakLine
        ? length
        : length + lineLength;

      if (sentence.IsSolved) Util.WriteInGreen(sentenceString);
      else Util.WriteInYellow(sentenceString);

      if (sentence != last)
      {
        Console.Write(separator);
        lineLength += separator.Length;
        continue;
      }

      Console.WriteLine(".");
    }
  }

  private static Coordinate GetOrigin(int width, int height) =>
    new((Console.WindowWidth - width) / 2, Console.CursorTop - height);

  private static void DrawCell(Cell cell, Coordinate origin)
  {
    Console.BackgroundColor = cell.Character.IsKnown
      ? ConsoleColor.DarkGray
      : ConsoleColor.Blue;

    Console.SetCursorPosition(origin.Row + (cell.Coordinate.Column * CellWidth), origin.Column + cell.Coordinate.Row);
    Console.Write($" {(cell.Character)} ".ToUpper());
    Console.ResetColor();
  }
}