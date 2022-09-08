using System.CommandLine;
using GodMode.Cli;
using GodMode.Cli.Settings;

var urlsSettings = SettingsReader.GetSection<UrlsSettings>("urls");
var godSettings = SettingsReader.GetSection<GodSettings>("god");
var cacheDirectory = SettingsReader.Get<string>("cacheDirectory");

var httpClient = new HttpClient();

var godWikiReader = new GodWikiReader(httpClient, urlsSettings, cacheDirectory);
var newspaperReader = new NewspaperReader(httpClient, urlsSettings, godSettings, cacheDirectory);

var rootCommand = new RootCommand("True GodMode for GV");
var issueOption = OptionsFactory.CreateIssueOption();
rootCommand.AddGlobalOption(issueOption);

var crosswordCommand = new Command("crossword", "Try to solve the crossword and print the result on screen.");
crosswordCommand.AddAlias("cw");
crosswordCommand.SetHandler(SolveCrossword(newspaperReader, godWikiReader), issueOption);

var knowYourMonsterCommand = new Command("know-your-monster", "Try to find out who is the monster of the day.");
knowYourMonsterCommand.AddAlias("kym");
knowYourMonsterCommand.SetHandler(() =>
{
  Util.WriteWarning("NotImplementedYet");
});

rootCommand.AddCommand(crosswordCommand);
rootCommand.AddCommand(knowYourMonsterCommand);

rootCommand.SetHandler(SolveCrossword(newspaperReader, godWikiReader), issueOption);

return await rootCommand.InvokeAsync(args);

Func<string, Task> SolveCrossword(NewspaperReader newspaperReader1, GodWikiReader godWikiReader1)
{
  return async issue =>
  {
    var crossword = await newspaperReader1.GetCrosswordAsync(issue);

    crossword.Solve((await godWikiReader1.GetOmnibusListAsync(issue)).ToArray());
    Drawer.DrawCrossword(crossword);
  };
}