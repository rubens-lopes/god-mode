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

rootCommand.SetHandler(async issue =>
{
  var crossword = await newspaperReader.GetCrosswordAsync(issue);

  crossword.Solve((await godWikiReader.GetOmnibusListAsync(issue)).ToArray());
  Drawer.DrawCrossword(crossword);
}, issueOption);

return await rootCommand.InvokeAsync(args);
