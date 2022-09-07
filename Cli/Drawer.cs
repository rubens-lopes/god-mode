using GodMode.Domain.Entities;

namespace GodMode.Cli;

public class Drawer
{
  private const int CELL_WIDTH = 3;

  public static void DrawCrossword(Crossword crossword)
  {
    var width = 0;
    if (crossword.LongestSentenceAcross != null)
      width = crossword.LongestSentenceAcross.Length * CELL_WIDTH;

    var origin = Centralize(width);

    foreach (var cell in crossword.Cells.ToArray())
      DrawCell(cell, origin);

    Console.WriteLine();
  }

  private static Coordinate Centralize(int width) =>
    new((Console.WindowWidth - width) / 2, Console.CursorTop);

  private static void DrawCell(Cell cell, Coordinate origin)
  {
    Console.BackgroundColor = cell.Character.IsKnown
      ? ConsoleColor.DarkGray
      : ConsoleColor.Blue;

    Console.SetCursorPosition(origin.Row + (cell.Coordinate.Column * CELL_WIDTH), origin.Column + cell.Coordinate.Row);
    Console.Write($" {(cell.Character)} ".ToUpper());
    Console.ResetColor();
  }
}