namespace GodMode.Domain.Entities;

public record MetaData(int Ordinal, int LocalOrdinal)
{
  public readonly int Ordinal = Ordinal;
  public readonly int LocalOrdinal = LocalOrdinal;
}