using Fizzler.Systems.HtmlAgilityPack;
using GodMode.Cli.Settings;
using HtmlAgilityPack;

namespace GodMode.Cli;

public interface IGodWikiReader
{
  Task<IEnumerable<string>> GetOmnibusListAsync(string issue);
}

public class GodWikiReader : IGodWikiReader
{
  private const string OmnibusExtension = "omnibus";

  private readonly HttpClient _httpClient;
  private readonly HtmlDocument _parser;
  private readonly UrlsSettings _urlsSettings;
  private readonly string _cacheDirectory;

  public GodWikiReader(HttpClient httpClient, UrlsSettings urlsSettings, string cacheDirectory)
  {
    _httpClient = httpClient;
    _parser = new HtmlDocument();
    _urlsSettings = urlsSettings;
    _cacheDirectory = cacheDirectory;
  }

  public async Task<IEnumerable<string>> GetOmnibusListAsync(string issue)
  {
    (bool success, IEnumerable<string> list) = await TryGetOmnibusListCacheAsync(issue);

    if (success) return list;

    var response = await _httpClient.GetAsync(_urlsSettings.Omnibus);

    _parser.LoadHtml(await response.Content.ReadAsStringAsync());

    var omnibusList = _parser.DocumentNode
      .QuerySelectorAll("#list-S, #list-E, #list-M, #list-A")
      .SelectMany(listNode => listNode.QuerySelectorAll("li"))
      .Select(itemNode => itemNode.InnerText)
      .ToArray();

    var result = (await GetTownsAsync())
      .Concat(omnibusList)
      .ToArray();

    await WriteOmnibusListCacheAsync(issue, result);

    return result;
  }

  private async Task<(bool sucsses, IEnumerable<string> list)> TryGetOmnibusListCacheAsync(string issue)
  {
    var path = $"./{_cacheDirectory}/{issue}.{OmnibusExtension}";

    var exists = File.Exists(path);

    if (exists == false) return (false, Array.Empty<string>());

    var list = await File.ReadAllLinesAsync(path);

    return (true, list);
  }

  private async Task WriteOmnibusListCacheAsync(string issue, IEnumerable<string> list)
  {
    var path = $"./{_cacheDirectory}/{issue}.{OmnibusExtension}";

    var exists = File.Exists(path);

    if (exists)
      File.Delete(path);

    await using StreamWriter file = new(path);

    foreach (string line in list)
      await file.WriteLineAsync(line);
  }

  private async Task<string[]> GetTownsAsync()
  {
    var response = await _httpClient.GetAsync(_urlsSettings.Towns);

    _parser.LoadHtml(await response.Content.ReadAsStringAsync());

    return _parser.DocumentNode
      .QuerySelectorAll("table.wikitable.sortable tbody th[scope=\"row\"] a")
      .Select(townNode => townNode.InnerText)
      .ToArray();
  }
}