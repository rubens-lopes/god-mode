using System.CommandLine;

namespace GodMode.Cli;

public static class OptionsFactory
{
  public static Option<string> CreateIssueOption() => new(
    aliases: new[] {"--issue", "-i"},
    description: "Newspaper issue date to work with",
    isDefault: true,
    parseArgument: result =>
    {
      var defaultArgument = DateTime.Now;
      if (defaultArgument <= new DateTime(defaultArgument.Year, defaultArgument.Month, defaultArgument.Day, 18, 5, 0))
        defaultArgument = defaultArgument.AddDays(-1);

      var argument = $"{defaultArgument:yyyy-MM-dd}";
      if (result.Tokens.Count == 0)
        return argument;

      var validDate = DateTime.TryParse(result.Tokens.Single().Value, out var issueDate);

      if (validDate == false)
      {
        Util.WriteWarning($"Issue date invalid, using default instead ({argument})...");
        return argument;
      }

      return $"{issueDate:yyyy-MM-dd}";
    }
  );
}