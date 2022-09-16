using GodMode.Cli.Settings;

namespace GodMode.Cli.Cache;

public interface ICacheProvider
{
  bool Has(string issue, Section section);
  Task WriteAsync(string issue, Section section, Stream stream);
  Stream Read(string issue, Section section);
}

public class CacheProvider : ICacheProvider
{
  private readonly string _homeDirectory;
  private readonly string _cacheDirectory;

  private string CachePath => $"{_homeDirectory}/{_cacheDirectory}/";

  public CacheProvider(ISettingsReader settingsReader)
  {
    _homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    _cacheDirectory = settingsReader.GetCacheDirectory();
  }

  public bool Has(string issue, Section section) => File.Exists($"{CachePath}{issue}{section.Extension}");

  public async Task WriteAsync(string issue, Section section, Stream stream)
  {
    if (Has(issue, section))
      File.Delete($"{CachePath}{issue}{section.Extension}");

    await using var fileStream = File.Create($"{CachePath}{issue}{section.Extension}");
    stream.Seek(0, SeekOrigin.Begin);
    await stream.CopyToAsync(fileStream);
    stream.Seek(0, SeekOrigin.Begin);
  }

  public Stream Read(string issue, Section section) => File.OpenRead($"{CachePath}{issue}{section.Extension}");
}