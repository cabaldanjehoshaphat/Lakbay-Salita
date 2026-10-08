using System.Collections.Generic;

/// <summary>
/// Hiligaynon_Level4
/// Hardcoded crossword puzzle data for Hiligaynon level 4 (4 across,
/// 4 down; 3-6 letter words). Grid position is 0-indexed (row 0 = top,
/// col 0 = left); any cell not covered by a word below is a black/blocked cell, derived
/// at runtime rather than stored here. See CrosswordPuzzleData.cs for the shared types.
/// </summary>
public static class Hiligaynon_Level4
{
    public static readonly CrosswordPuzzle Data = new CrosswordPuzzle("Hiligaynon", 4, new List<CrosswordWordEntry>
    {
        new CrosswordWordEntry(1, "BALAY", "This is a building where people live.", 0, 1, CrosswordDirection.Across),
        new CrosswordWordEntry(2, "ATO", "This word points to something that is farther away from the speaker.", 0, 2, CrosswordDirection.Down),
        new CrosswordWordEntry(3, "AGAS", "This is an oil-based fuel used for lamps and heating.", 0, 4, CrosswordDirection.Down),
        new CrosswordWordEntry(4, "KOLAR", "This is the part of a shirt that fits around the neck.", 2, 1, CrosswordDirection.Across),
        new CrosswordWordEntry(4, "KESO", "This is a dairy food made from curdled milk.", 2, 1, CrosswordDirection.Down),
        new CrosswordWordEntry(5, "POTOK", "This is a tight twist or bend in something like a rope or hair.", 5, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(6, "TAY", "This is a familiar, affectionate term for one's father.", 5, 2, CrosswordDirection.Down),
        new CrosswordWordEntry(7, "ALYAK", "This means to let a liquid accidentally flow out of its container.", 7, 0, CrosswordDirection.Across),
    });
}
