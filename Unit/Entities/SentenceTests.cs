using FluentAssertions;
using GodMode.Domain.Entities;

namespace GodMode.Tests.Unit.Entities;

public class SentenceTests
{
  [Fact]
  public void Add_should_append_new_characters_to_the_sentence()
  {
    var sentence = new Sentence(new Character('U'));

    var characters = "nit test!"
      .Select(character => new Character(character))
      .ToArray();

    sentence.Add(characters);

    sentence.ToString()
      .Should()
      .Be("Unit test!");
  }

  [Fact]
  public void Sentence_should_be_solved_after_TryToSolve_succeeds()
  {
    Character[] characters =
    {
      new('H'), new(), new('l'), new(), new(), new(' '), new(), new('e'), new(), new('t'), new('!'),
    };

    var sentence = new Sentence(characters);

    sentence.TryToSolve(new[] {"Hello, world!", "Olá word", "Olá test!", "Hello test!"});

    sentence.IsSolved
      .Should()
      .BeTrue();
  }

  [Fact]
  public void TryToSolve_should_return_false_if_it_has_more_than_one_answer()
  {
    Character[] characters = {new('H'), new(), new('l'), new(), new(), new(' '), new('w'), new('o'), new(), new('d')};

    var sentence = new Sentence(characters);

    var result = sentence.TryToSolve(new[] {"Hello, world!", "Hello word", "Hello wood"});

    result
      .Should()
      .BeFalse();
  }
  
  [Fact]
  public void TryToSolve_should_return_false_if_it_has_no_answer()
  {
    Character[] characters = {new('H'), new(), new('l'), new(), new(), new(' '), new('w'), new('o'), new(), new('d')};

    var sentence = new Sentence(characters);

    var result = sentence.TryToSolve(Array.Empty<string>());

    result
      .Should()
      .BeFalse();
  }
}