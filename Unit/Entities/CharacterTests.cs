using FluentAssertions;
using GodMode.Domain.Entities;

namespace GodMode.Tests.Unit.Entities;

public class CharacterTests
{
  [Fact]
  public void Character_should_still_be_unknown_after_been_set()
  {
    var character = new Character();
    character.Set('R');

    character
      .IsKnown
      .Should()
      .BeFalse();
  }

  [Fact]
  public void New_Character_should_be_known()
  {
    var character = new Character('c');

    character
      .IsKnown
      .Should()
      .BeTrue();
  }

  [Fact]
  public void ToString_should_return_empty_string_when_not_known()
  {
    var character = new Character();

    character.ToString()
      .Should()
      .Be("_");
  }
}