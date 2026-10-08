using System.Collections.Generic;

/// <summary>
/// Tagalog_Level5
/// Hardcoded crossword puzzle data for Tagalog level 5 (5 across,
/// 4 down; 4-7 letter words). Grid position is 0-indexed (row 0 = top,
/// col 0 = left); any cell not covered by a word below is a black/blocked cell, derived
/// at runtime rather than stored here. See CrosswordPuzzleData.cs for the shared types.
/// </summary>
public static class Tagalog_Level5
{
    public static readonly CrosswordPuzzle Data = new CrosswordPuzzle("Tagalog", 5, new List<CrosswordWordEntry>
    {
        new CrosswordWordEntry(1, "HULA", "This is a prediction of what will happen, especially about the weather.", 0, 3, CrosswordDirection.Across),
        new CrosswordWordEntry(1, "HIKA", "This is a condition that makes breathing difficult due to narrowed airways.", 0, 3, CrosswordDirection.Down),
        new CrosswordWordEntry(2, "LALIKIN", "This means to shape an object by spinning it against a cutting tool.", 0, 5, CrosswordDirection.Down),
        new CrosswordWordEntry(3, "ANAK", "This is a person's son or daughter.", 2, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(4, "AGIW", "This is the black powdery residue left behind by burning something.", 3, 3, CrosswordDirection.Across),
        new CrosswordWordEntry(5, "PULI", "This is something used to take the place of another.", 5, 2, CrosswordDirection.Across),
        new CrosswordWordEntry(5, "PIPI", "This describes someone who is unable to speak.", 5, 2, CrosswordDirection.Down),
        new CrosswordWordEntry(6, "NOOD", "This means to watch a performance or program.", 5, 7, CrosswordDirection.Down),
        new CrosswordWordEntry(7, "IBILAD", "This means to lay something outside so the sun's heat dries it.", 8, 2, CrosswordDirection.Across),
    });
}
