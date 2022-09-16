using Microsoft.Extensions.Configuration;

namespace GodMode.Cli.Settings;

public interface ISettingsReader
{
  bool IsDefaultDate(string date);
  string GetDefaultDate();
  string AsDefaultDate(DateTime issueDate);
  public bool IsLesserOrEqualThanDefaultDate(DateTime date);
  public UrlsSettings GetUrlsSettings();
  public GodSettings GetGodSettings();
  public string GetCacheDirectory();
}

public sealed class SettingsReader : ISettingsReader
{
  public bool IsDefaultDate(string date) => GetDefaultDate() == date;

  public bool IsLesserOrEqualThanDefaultDate(DateTime date) =>
    String.CompareOrdinal(AsDefaultDate(date), GetDefaultDate()) <= 0;

  public string GetDefaultDate()
  {
    var defaultDate = DateTime.Now;
    if (defaultDate <= new DateTime(defaultDate.Year, defaultDate.Month, defaultDate.Day, 18, 5, 0))
      defaultDate = defaultDate.AddDays(-1);

    return AsDefaultDate(defaultDate);
  }
  public string AsDefaultDate(DateTime issueDate) => $"{issueDate:yyyy-MM-dd}";

  public UrlsSettings GetUrlsSettings() => GetSection<UrlsSettings>("urls");

  public GodSettings GetGodSettings() => GetSection<GodSettings>("god");
  public string GetCacheDirectory() => Get<string>("cacheDirectory");

  private static T Get<T>(string value) => GetSettings()
    .GetValue<T>(value);

  private static T GetSection<T>(string section) => GetSettings()
    .GetSection(section)
    .Get<T>();

  private static IConfigurationRoot GetSettings()
  {
    var environment = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");

    var settings = new ConfigurationBuilder()
      .AddJsonFile("./appsettings.json", optional: true)
      .AddJsonFile($"./appsettings.{environment}.json", optional: true)
      .AddUserSecrets<Program>()
      .AddEnvironmentVariables()
      .Build();

    return settings;
  }
}