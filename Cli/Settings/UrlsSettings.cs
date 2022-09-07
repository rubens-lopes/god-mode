// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global

#pragma warning disable CS8618
namespace GodMode.Cli.Settings;

public record UrlsSettings
{
  public string Newspaper { get; set; }
  public string Towns { get; set; }
  public string Login { get; set; }
  public string Omnibus { get; set; }
}