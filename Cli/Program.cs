using System.CommandLine;
using GodMode.Cli;
using GodMode.Cli.Cache;
using GodMode.Cli.Settings;

var urlsSettings = SettingsReader.GetSection<UrlsSettings>("urls");
var godSettings = SettingsReader.GetSection<GodSettings>("god");
var cacheDirectory = SettingsReader.Get<string>("cacheDirectory");
var httpClient = new HttpClient();

var rootCommand = new RootCommand("True GodMode for GV");
var issueOption = OptionsFactory.CreateIssueOption();
rootCommand.AddGlobalOption(issueOption);

var crosswordCommand = new Command("crossword", "Try to solve the crossword and print the result on screen.");
crosswordCommand.AddAlias("cw");
crosswordCommand.SetHandler(async issue => await SolveCrossword(issue), issueOption);

var knowYourMonsterCommand = new Command("monster-of-the-day", "Try to find out who is the monster of the day.");
knowYourMonsterCommand.AddAlias("md");
knowYourMonsterCommand.SetHandler(async issue =>
{
  var cacheProvider = new CacheProvider(cacheDirectory, issue);
  var newspaperReader = new NewspaperReader(httpClient, urlsSettings, godSettings, cacheProvider);
  await newspaperReader.GetMonsterAsync(issue);
}, issueOption);

rootCommand.AddCommand(crosswordCommand);
rootCommand.AddCommand(knowYourMonsterCommand);

rootCommand.SetHandler(async issue => await SolveCrossword(issue), issueOption);

return await rootCommand.InvokeAsync(args);

async Task SolveCrossword(string issue)
{
  var cacheProvider = new CacheProvider(cacheDirectory, issue);

  var godWikiReader = new GodWikiReader(httpClient, urlsSettings, cacheProvider);
  var newspaperReader = new NewspaperReader(httpClient, urlsSettings, godSettings, cacheProvider);

  var crossword = await newspaperReader.GetCrosswordAsync(issue);

  if (crossword == null)
  {
    Util.WriteWarning("Unable to get crossword");
    return;
  }

  crossword.Solve((await godWikiReader.GetOmnibusListAsync(issue)).ToArray());
  Drawer.DrawCrossword(crossword);
}