namespace GodMode.Domain.Entities;

public record Cell(Coordinate Coordinate)
{
  public readonly Coordinate Coordinate = Coordinate;
  public readonly Character Character = new();

  public Cell(Coordinate coordinate, char character) : this(coordinate) => Character = new Character(character);
}