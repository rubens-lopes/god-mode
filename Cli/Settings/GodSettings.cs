// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global

#pragma warning disable CS8618
namespace GodMode.Cli.Settings;

public record GodSettings
{
  public string Name { get; set; }
  public string Password { get; set; }
}