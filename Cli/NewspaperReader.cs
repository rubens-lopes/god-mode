using GodMode.Domain.Entities;
using Fizzler.Systems.HtmlAgilityPack;
using GodMode.Cli.Cache;
using GodMode.Cli.Settings;
using HtmlAgilityPack;

namespace GodMode.Cli;

public interface INewsPaperReader
{
  Task<Crossword?> GetCrosswordAsync(string issue);
  Task GetMonsterAsync(string issue);
}

public class NewspaperReader : INewsPaperReader
{
  private readonly HtmlDocument _parser;
  private readonly HttpClient _httpClient;
  private readonly UrlsSettings _urlsSettings;
  private readonly GodSettings _godSettings;
  private readonly CacheProvider _cacheProvider;

  public NewspaperReader(HttpClient httpClient, UrlsSettings urlsSettings, GodSettings godSettings,
    CacheProvider cacheProvider)
  {
    _httpClient = httpClient;
    _parser = new HtmlDocument();
    _urlsSettings = urlsSettings;
    _godSettings = godSettings;
    _cacheProvider = cacheProvider;
  }

  public async Task<Crossword?> GetCrosswordAsync(string issue)
  {
    Stream contentStream;
    var hasCache = _cacheProvider.Has(Section.Newspaper);

    if (hasCache)
      contentStream = _cacheProvider.Read(Section.Newspaper);
    else if (SettingsReader.IsDefaultDate(issue) == false)
      return null;
    else
    {
      if (await IsLoggedIdAsync() == false)
        await LogInAsync();

      var response = await _httpClient.GetAsync(_urlsSettings.Newspaper);
      contentStream = await response.Content.ReadAsStreamAsync();
      await _cacheProvider.WriteAsync(Section.OmnibusList, contentStream);
    }

    _parser.Load(contentStream);

    var cells = _parser.DocumentNode
      .QuerySelector("#cross_tbl")
      .QuerySelectorAll("tr")
      .SelectMany((rowNode, row) => rowNode
        .QuerySelectorAll("td")
        .Where(cellNode => cellNode.HasClass("cc_wrap") == false)
        .Select((cellNode, column) => new {cellNode, coordinate = new Coordinate(row, column)}))
      .Select(x =>
      {
        if (x.cellNode.HasClass("td_cell") == false) return null;

        if (x.cellNode.HasClass("known") == false) return new Cell(x.coordinate);

        var character = char.Parse(x.cellNode
          .QuerySelector("div.open")
          .InnerText);

        return new Cell(x.coordinate, character);
      })
      .Where(cell => cell != null)
      .ToArray();

    return new Crossword(cells!);
  }

  public async Task GetMonsterAsync(string issue)
  {
    var hasCache = _cacheProvider.Has(Section.MonsterOfTheDay);

    if (hasCache) return;

    if (SettingsReader.IsDefaultDate(issue) == false) return;

    if (await IsLoggedIdAsync() == false)
      await LogInAsync();

    var response = await _httpClient.GetAsync(_urlsSettings.MonsterOfTheDay);
    var contentStream = await response.Content.ReadAsStreamAsync();
    await _cacheProvider.WriteAsync(Section.MonsterOfTheDay, contentStream);
  }

  private async Task<bool> IsLoggedIdAsync()
  {
    var response = await _httpClient.GetAsync(_urlsSettings.Newspaper);
    var content = await response.Content.ReadAsStringAsync();

    _parser.LoadHtml(content);

    var isLoggedId = _parser.DocumentNode
      .QuerySelector("a[href=\"/login/register\"]") == null;

    return isLoggedId;
  }

  private async Task LogInAsync()
  {
    var request = new HttpRequestMessage
    {
      Method = HttpMethod.Post,
      RequestUri = new Uri(_urlsSettings.Login),
      Content = new FormUrlEncodedContent(new[]
      {
        new KeyValuePair<string, string>("username", _godSettings.Name),
        new KeyValuePair<string, string>("password", _godSettings.Password),
        new KeyValuePair<string, string>("save_login", "false"),
        new KeyValuePair<string, string>("commit", "Login"),
      }),
    };
    request.Headers.Add("Cache-Control", "no-cache");
    request.Headers.Add("Pragma", "no-cache");
    request.Headers.Add("User-Agent",
      "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:104.0) Gecko/20100101 Firefox/104.0");

    await _httpClient.SendAsync(request);
  }
}