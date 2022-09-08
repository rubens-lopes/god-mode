using System.CommandLine;
using GodMode.Cli.Settings;

namespace GodMode.Cli;

public static class OptionsFactory
{
  public static Option<string> CreateIssueOption() => new(
    aliases: new[] {"--issue", "-i"},
    description: "Newspaper issue date to work with",
    isDefault: true,
    parseArgument: result =>
    {
      var defaultArgument = SettingsReader.GetDefaultDate();

      if (result.Tokens.Count == 0)
        return defaultArgument;

      var isValidDate = DateTime.TryParse(result.Tokens.Single().Value, out var issueDate);

      if (isValidDate && SettingsReader.IsLesserOrEqualThanDefaultDate(issueDate))
        return SettingsReader.AsDefaultDate(issueDate);

      Util.WriteWarning($"Issue date invalid, using default instead ({defaultArgument})...");
      return defaultArgument;
    }
  );
}