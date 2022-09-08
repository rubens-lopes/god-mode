using Microsoft.Extensions.Configuration;

namespace GodMode.Cli.Settings;

public static class SettingsReader
{
  public static bool IsDefaultDate(string date) => GetDefaultDate() == date;

  public static bool IsLesserOrEqualThanDefaultDate(DateTime date) =>
    String.CompareOrdinal(AsDefaultDate(date), GetDefaultDate()) <= 0;

  public static string GetDefaultDate()
  {
    var defaultDate = DateTime.Now;
    if (defaultDate <= new DateTime(defaultDate.Year, defaultDate.Month, defaultDate.Day, 18, 5, 0))
      defaultDate = defaultDate.AddDays(-1);

    return AsDefaultDate(defaultDate);
  }

  public static T Get<T>(string value) => GetSettings()
    .GetValue<T>(value);

  public static T GetSection<T>(string section) => GetSettings()
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

  public static string AsDefaultDate(DateTime issueDate) => $"{issueDate:yyyy-MM-dd}";
}