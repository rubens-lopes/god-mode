using GodMode.Domain.Entities;

namespace GodMode.Cli;

public class Drawer
{
  private const int CELL_WIDTH = 3;

  public static void DrawCrossword(Crossword crossword)
  {
    var width = crossword.LongestAcrossSentence.Length * CELL_WIDTH;
    var height = crossword.LongestDownSentence.Length;
    
    for (var line = 0; line < height; line++)
      Console.WriteLine();

    var origin = SetOrigin(width, height);

    foreach (var cell in crossword.Cells.ToArray())
      DrawCell(cell, origin);

    Console.WriteLine();
  }

  private static Coordinate SetOrigin(int width, int height) =>
    new((Console.WindowWidth - width) / 2, Console.CursorTop - height);

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