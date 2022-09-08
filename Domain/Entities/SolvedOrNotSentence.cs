namespace GodMode.Domain.Entities;

public record SolvedOrNotSentence
{
  private readonly int _ordinal;
  private readonly Sentence _sentence;

  public SolvedOrNotSentence(int ordinal, Sentence sentence)
  {
    _ordinal = ordinal;
    _sentence = sentence;
  }

  public bool IsSolved => _sentence.IsSolved;
  
  public override string ToString() => $"{_ordinal}. {_sentence}";
}