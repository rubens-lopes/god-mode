using System.CommandLine;
using GodMode.Cli;
using GodMode.Cli.Cache;
using GodMode.Cli.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using IHost host = Host.CreateDefaultBuilder(args)
  .ConfigureServices((_, services) => services
    .AddSingleton<HttpClient, HttpClient>()
    .AddScoped<IGodWikiReader, GodWikiReader>()
    .AddScoped<ICacheProvider, CacheProvider>()
    .AddScoped<ISettingsReader, SettingsReader>()
    .AddScoped<INewspaperReader, NewspaperReader>()
  )
  .Build();

var serviceProvider = host.Services.CreateAsyncScope().ServiceProvider;
var settingsReader = serviceProvider.GetRequiredService<ISettingsReader>();

var rootCommand = new RootCommand("True GodMode for GV");
var issueOption = new OptionsFactory(settingsReader).CreateIssueOption();
rootCommand.AddGlobalOption(issueOption);

var crosswordCommand = new Command("crossword", "Try to solve the crossword and print the result on screen.");
crosswordCommand.AddAlias("cw");
crosswordCommand.SetHandler(async issue => await SolveCrossword(issue), issueOption);

var knowYourMonsterCommand = new Command("monster-of-the-day", "Try to find out who is the monster of the day.");
knowYourMonsterCommand.AddAlias("md");
knowYourMonsterCommand.SetHandler(async issue => await SolveMonsterOfTheDay(issue), issueOption);

rootCommand.AddCommand(crosswordCommand);
rootCommand.AddCommand(knowYourMonsterCommand);

rootCommand.SetHandler(async issue => await Task
  .WhenAll(SolveCrossword(issue), SolveMonsterOfTheDay(issue)), issueOption);

return await rootCommand.InvokeAsync(args);

async Task SolveCrossword(string issue)
{
  var godWikiReader = serviceProvider.GetRequiredService<IGodWikiReader>();
  var newspaperReader = serviceProvider.GetRequiredService<INewspaperReader>();

  var crossword = await newspaperReader.GetCrosswordAsync(issue);

  if (crossword == null)
  {
    Util.WriteWarning("Unable to get crossword");
    return;
  }

  crossword.Solve((await godWikiReader.GetOmnibusListAsync(issue)).ToArray());
  Drawer.DrawCrossword(crossword);
}

async Task SolveMonsterOfTheDay(string issue)
{
  var newspaperReader = serviceProvider.GetRequiredService<INewspaperReader>();
  await newspaperReader.GetMonsterAsync(issue);
}