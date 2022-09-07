using Microsoft.Extensions.Configuration;

namespace GodMode.Cli.Settings;

public static class SettingsReader
{
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
}