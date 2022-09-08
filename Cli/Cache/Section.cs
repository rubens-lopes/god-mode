using Ardalis.SmartEnum;

namespace GodMode.Cli.Cache;

public sealed class Section : SmartEnum<Section, ushort>
{
  public readonly string Extension;

  public static readonly Section Newspaper = new(nameof(Newspaper), 1);
  public static readonly Section OmnibusList = new(nameof(OmnibusList), 2);
  public static readonly Section MonsterOfTheDay = new(nameof(MonsterOfTheDay), 3);
  public static readonly Section Towns = new(nameof(Towns), 4);

  private Section(string name, ushort value) : base(name, value)
  {
    Extension = $".{name}".ToLower();
  }
}