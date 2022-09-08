namespace GodMode.Cli;

public static class Util
{
  public static void WriteWarning(string warning)
  {
    Console.ForegroundColor = ConsoleColor.DarkYellow;
    Console.WriteLine(warning);
    Console.ResetColor();
  }

  public static void WriteInYellow(string message) => WriteInColor(message, ConsoleColor.DarkYellow);
  public static void WriteInGreen(string message) => WriteInColor(message, ConsoleColor.DarkGreen);
  public static void WriteInMagenta(string message) => WriteInColor(message, ConsoleColor.Magenta);

  private static void WriteInColor(string message, ConsoleColor color)
  {
    Console.ForegroundColor = color;
    Console.Write(message);
    Console.ResetColor();
  }
}