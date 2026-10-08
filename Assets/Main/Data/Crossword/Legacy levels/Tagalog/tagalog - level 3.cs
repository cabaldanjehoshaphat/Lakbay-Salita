using System.Collections.Generic;

/// <summary>
/// Tagalog_Level3
/// Hardcoded crossword puzzle data for Tagalog level 3 (4 across,
/// 3 down; 3-6 letter words). Grid position is 0-indexed (row 0 = top,
/// col 0 = left); any cell not covered by a word below is a black/blocked cell, derived
/// at runtime rather than stored here. See CrosswordPuzzleData.cs for the shared types.
/// </summary>
public static class Tagalog_Level3
{
    public static readonly CrosswordPuzzle Data = new CrosswordPuzzle("Tagalog", 3, new List<CrosswordWordEntry>
    {
        new CrosswordWordEntry(1, "TUMAWA", "This is the sound and expression people make when something is funny.", 0, 1, CrosswordDirection.Down),
        new CrosswordWordEntry(2, "UMAT", "This describes taking a long time to do or finish something.", 2, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(3, "TAPA", "This is thin meat that has been cured and dried for preservation.", 2, 3, CrosswordDirection.Down),
        new CrosswordWordEntry(4, "ADIK", "This is a person who cannot stop using a substance or habit.", 2, 5, CrosswordDirection.Across),
        new CrosswordWordEntry(5, "DIES", "This is the number that comes after nine.", 2, 6, CrosswordDirection.Down),
        new CrosswordWordEntry(6, "PUWES", "This word is used to introduce a conclusion drawn from what was just said.", 4, 3, CrosswordDirection.Across),
        new CrosswordWordEntry(7, "LAPA", "This is the act of cutting up an animal's carcass into meat.", 5, 0, CrosswordDirection.Across),
    });
}
