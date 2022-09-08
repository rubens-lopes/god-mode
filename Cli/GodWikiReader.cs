using Fizzler.Systems.HtmlAgilityPack;
using GodMode.Cli.Cache;
using GodMode.Cli.Settings;
using HtmlAgilityPack;

namespace GodMode.Cli;

public interface IGodWikiReader
{
  Task<IEnumerable<string>> GetOmnibusListAsync(string issue);
}

public class GodWikiReader : IGodWikiReader
{
  private readonly HttpClient _httpClient;
  private readonly HtmlDocument _parser;
  private readonly UrlsSettings _urlsSettings;
  private readonly CacheProvider _cacheProvider;

  public GodWikiReader(HttpClient httpClient, UrlsSettings urlsSettings, CacheProvider cacheProvider)
  {
    _httpClient = httpClient;
    _parser = new HtmlDocument();
    _urlsSettings = urlsSettings;
    _cacheProvider = cacheProvider;
  }

  public async Task<IEnumerable<string>> GetOmnibusListAsync(string issue)
  {
    Stream contentStream;
    var hasCache = _cacheProvider.Has(Section.OmnibusList);

    if (hasCache)
      contentStream = _cacheProvider.Read(Section.OmnibusList);
    else if (SettingsReader.IsDefaultDate(issue) == false)
      return Array.Empty<string>();
    else
    {
      var response = await _httpClient.GetAsync(_urlsSettings.Omnibus);
      contentStream = await response.Content.ReadAsStreamAsync();
      await _cacheProvider.WriteAsync(Section.OmnibusList, contentStream);
    }

    _parser.Load(contentStream);

    var omnibusList = _parser.DocumentNode
      .QuerySelectorAll("#list-S, #list-E, #list-M, #list-A")
      .SelectMany(listNode => listNode.QuerySelectorAll("li"))
      .Select(itemNode => itemNode.InnerText)
      .ToArray();

    var result = (await GetTownsAsync(issue))
      .Concat(omnibusList)
      .ToArray();

    return result;
  }

  private async Task<string[]> GetTownsAsync(string issue)
  {
    Stream contentStream;
    var hasCache = _cacheProvider.Has(Section.Towns);

    if (hasCache)
      contentStream = _cacheProvider.Read(Section.Towns);
    else if (SettingsReader.IsDefaultDate(issue) == false)
      return Array.Empty<string>();
    else
    {
      var response = await _httpClient.GetAsync(_urlsSettings.Towns);
      contentStream = await response.Content.ReadAsStreamAsync();
      await _cacheProvider.WriteAsync(Section.Towns, contentStream);
    }

    _parser.Load(contentStream);

    return _parser.DocumentNode
      .QuerySelectorAll("table.wikitable.sortable tbody th[scope=\"row\"] a")
      .Select(townNode => townNode.InnerText)
      .ToArray();
  }
}