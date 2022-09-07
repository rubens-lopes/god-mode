namespace GodMode.Domain.Entities;

public record Character()
{
  private char? Value { get; set; }
  public bool HasValue => Value.HasValue;
  public readonly bool IsKnown;

  public Character(char character) : this()
  {
    IsKnown = true;
    Value = character;
  }

  public void Set(char character) => Value = character;

  public override string ToString() => Value.HasValue
    ? Value.Value.ToString()
    : "_";

  public string ToRegexToken()
  {
    if (Value.HasValue == false) return ".";

    var str = Value.Value.ToString();
    return str switch
    {
      " " => @"\s",
      _ => str,
    };
  }
}