using GodMode.Domain.Entities;

namespace GodMode.Domain.Extensions;

public static class CoordinateExtensions
{
  public static bool IsStartingDownSentence(this Coordinate coordinate, Cell[] cells) =>
    coordinate.HasTopCell(cells) == false && coordinate.HasBottomCell(cells);

  public static bool IsStartingAcrossSentence(this Coordinate coordinate, Cell[] cells) =>
    coordinate.HasStartCell(cells) == false && coordinate.HasEndCell(cells);

  public static bool IsStartOfSentence(this Coordinate coordinate, Cell[] cells) =>
    coordinate.IsStartingDownSentence(cells) || coordinate.IsStartingAcrossSentence(cells);

  public static bool IsPartialSentence(this Coordinate coordinate, Cell[] cells) =>
    coordinate.HasTopCell(cells) || coordinate.HasStartCell(cells);

  public static bool HasTopCell(this Coordinate coordinate, IEnumerable<Cell> cells)
  {
    var topCoordinate = coordinate with {Row = coordinate.Row - 1};

    return cells.Any(cell => cell.Coordinate == topCoordinate);
  }

  private static bool HasEndCell(this Coordinate coordinate, IEnumerable<Cell> cells)
  {
    var endCoordinate = coordinate with {Column = coordinate.Column + 1};

    return cells.Any(cell => cell.Coordinate == endCoordinate);
  }

  private static bool HasBottomCell(this Coordinate coordinate, IEnumerable<Cell> cells)
  {
    var bottomCoordinate = coordinate with {Row = coordinate.Row + 1};

    return cells.Any(cell => cell.Coordinate == bottomCoordinate);
  }

  public static bool HasStartCell(this Coordinate coordinate, IEnumerable<Cell> cells)
  {
    var startCoordinate = coordinate with {Column = coordinate.Column - 1};

    return cells.Any(cell => cell.Coordinate == startCoordinate);
  }
}