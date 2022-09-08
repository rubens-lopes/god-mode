namespace GodMode.Cli.Cache;

public class CacheProvider
{
  private readonly string _homeDirectory;
  private readonly string _cacheDirectory;
  private readonly string _issue;

  private string CachePath => $"{_homeDirectory}/{_cacheDirectory}/";

  public CacheProvider(string cacheDirectory, string issue)
  {
    _homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    _cacheDirectory = cacheDirectory;
    _issue = issue;
  }

  public bool Has(Section section) => File.Exists($"{CachePath}{_issue}{section.Extension}");

  public async Task WriteAsync(Section section, Stream stream)
  {
    if (Has(section))
      File.Delete($"{CachePath}{_issue}{section.Extension}");

    await using var fileStream = File.Create($"{CachePath}{_issue}{section.Extension}");
    stream.Seek(0, SeekOrigin.Begin);
    await stream.CopyToAsync(fileStream);
    stream.Seek(0, SeekOrigin.Begin);
  }

  public Stream Read(Section section) => File.OpenRead($"{CachePath}{_issue}{section.Extension}");
}