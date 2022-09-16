using System.CommandLine;
using GodMode.Cli.Settings;

namespace GodMode.Cli;

public sealed class OptionsFactory
{
  private readonly ISettingsReader _settingsReader;
  
  public OptionsFactory(ISettingsReader settingsReader)
  {
    _settingsReader = settingsReader;
  }
  
  public Option<string> CreateIssueOption() => new(
    aliases: new[] {"--issue", "-i"},
    description: "Newspaper issue date to work with",
    isDefault: true,
    parseArgument: result =>
    {
      var defaultArgument = _settingsReader.GetDefaultDate();

      if (result.Tokens.Count == 0)
        return defaultArgument;

      var isValidDate = DateTime.TryParse(result.Tokens.Single().Value, out var issueDate);

      if (isValidDate && _settingsReader.IsLesserOrEqualThanDefaultDate(issueDate))
        return _settingsReader.AsDefaultDate(issueDate);

      Util.WriteWarning($"Issue date invalid, using default instead ({defaultArgument})...");
      return defaultArgument;
    }
  );
}