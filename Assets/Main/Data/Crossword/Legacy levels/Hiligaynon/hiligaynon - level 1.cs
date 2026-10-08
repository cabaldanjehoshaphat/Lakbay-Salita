using System.Collections.Generic;

/// <summary>
/// Hiligaynon_Level1
/// Hardcoded crossword puzzle data for Hiligaynon level 1 (3 across,
/// 2 down; 3-5 letter words). Grid position is 0-indexed (row 0 = top,
/// col 0 = left); any cell not covered by a word below is a black/blocked cell, derived
/// at runtime rather than stored here. See CrosswordPuzzleData.cs for the shared types.
/// </summary>
public static class Hiligaynon_Level1
{
    public static readonly CrosswordPuzzle Data = new CrosswordPuzzle("Hiligaynon", 1, new List<CrosswordWordEntry>
    {
        new CrosswordWordEntry(1, "OROB", "This means to forcefully expel liquid from the mouth.", 0, 2, CrosswordDirection.Across),
        new CrosswordWordEntry(2, "REUMA", "This is a condition causing pain and stiffness in the joints and muscles.", 0, 3, CrosswordDirection.Down),
        new CrosswordWordEntry(3, "KUYUS", "This is a writing tool, traditionally made from a bird's feather.", 2, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(4, "USA", "This is the female of the bear species.", 2, 1, CrosswordDirection.Down),
        new CrosswordWordEntry(5, "KAWAD", "This is a single ring of a chain that connects to the others.", 4, 0, CrosswordDirection.Across),
    });
}
