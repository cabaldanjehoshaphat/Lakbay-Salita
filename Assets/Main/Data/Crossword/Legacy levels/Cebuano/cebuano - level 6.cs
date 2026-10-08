using System.Collections.Generic;

/// <summary>
/// Cebuano_Level6
/// Hardcoded crossword puzzle data for Cebuano level 6 (5 across,
/// 5 down; 4-7 letter words). Grid position is 0-indexed (row 0 = top,
/// col 0 = left); any cell not covered by a word below is a black/blocked cell, derived
/// at runtime rather than stored here. See CrosswordPuzzleData.cs for the shared types.
/// </summary>
public static class Cebuano_Level6
{
    public static readonly CrosswordPuzzle Data = new CrosswordPuzzle("Cebuano", 6, new List<CrosswordWordEntry>
    {
        new CrosswordWordEntry(1, "PAHUY", "This is a figure made to look like a person, set up in a field to scare away birds.", 0, 3, CrosswordDirection.Down),
        new CrosswordWordEntry(2, "DYAM", "This is a sweet spread made by cooking fruit with sugar.", 1, 1, CrosswordDirection.Across),
        new CrosswordWordEntry(3, "KINU", "This means to move something quickly back and forth or up and down.", 3, 1, CrosswordDirection.Down),
        new CrosswordWordEntry(4, "ISYU", "This is one numbered edition of a magazine or newspaper.", 4, 1, CrosswordDirection.Across),
        new CrosswordWordEntry(5, "UYAB", "This is the term for someone's romantic partner.", 6, 1, CrosswordDirection.Across),
        new CrosswordWordEntry(6, "AWLA", "This is a type of fish found in local waters.", 6, 3, CrosswordDirection.Down),
        new CrosswordWordEntry(7, "TIKI", "This is a small lizard often seen clinging to walls and ceilings.", 8, 1, CrosswordDirection.Down),
        new CrosswordWordEntry(8, "ILAK", "This describes a woman who is very attractive.", 9, 1, CrosswordDirection.Across),
        new CrosswordWordEntry(9, "KIMPI", "This describes someone whose knees touch while their ankles stay apart when standing.", 9, 4, CrosswordDirection.Down),
        new CrosswordWordEntry(10, "DIGUM", "This describes something that is completely black in color.", 11, 0, CrosswordDirection.Across),
    });
}
