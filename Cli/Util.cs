namespace GodMode.Cli;

public static class Util
{
  public static void WriteWarning(string warning)
  {
    Console.ForegroundColor = ConsoleColor.DarkYellow;
    Console.WriteLine(warning);
    Console.ResetColor();
  }
}