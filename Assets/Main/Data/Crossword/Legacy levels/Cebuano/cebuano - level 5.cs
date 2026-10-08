using System.Collections.Generic;

/// <summary>
/// Cebuano_Level5
/// Hardcoded crossword puzzle data for Cebuano level 5 (5 across,
/// 4 down; 4-7 letter words). Grid position is 0-indexed (row 0 = top,
/// col 0 = left); any cell not covered by a word below is a black/blocked cell, derived
/// at runtime rather than stored here. See CrosswordPuzzleData.cs for the shared types.
/// </summary>
public static class Cebuano_Level5
{
    public static readonly CrosswordPuzzle Data = new CrosswordPuzzle("Cebuano", 5, new List<CrosswordWordEntry>
    {
        new CrosswordWordEntry(1, "IPATONG", "This describes something that is going to be placed on top of or included with another.", 0, 3, CrosswordDirection.Across),
        new CrosswordWordEntry(2, "GRATIS", "This means something is given for free, without any charge.", 0, 9, CrosswordDirection.Down),
        new CrosswordWordEntry(3, "TAPLI", "This means to block or deflect an opponent's attack or blow.", 2, 3, CrosswordDirection.Down),
        new CrosswordWordEntry(4, "TUGA", "This means to formally give something as a gift or honor.", 2, 6, CrosswordDirection.Across),
        new CrosswordWordEntry(4, "TULUN", "This is the act of moving food or drink down the throat.", 2, 6, CrosswordDirection.Down),
        new CrosswordWordEntry(5, "MILAGRU", "This is an extraordinary event believed to be caused by a divine power.", 3, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(6, "IITU", "This is the common name for a whiskered freshwater fish.", 3, 1, CrosswordDirection.Down),
        new CrosswordWordEntry(7, "LUBI", "This is the scientific name for the tropical palm tree that produces coconuts.", 4, 6, CrosswordDirection.Across),
        new CrosswordWordEntry(8, "SULIRAN", "This is a difficult situation that needs to be solved.", 6, 0, CrosswordDirection.Across),
    });
}
