using System.Collections.Generic;

/// <summary>
/// Cebuano_Level4
/// Hardcoded crossword puzzle data for Cebuano level 4 (4 across,
/// 4 down; 3-6 letter words). Grid position is 0-indexed (row 0 = top,
/// col 0 = left); any cell not covered by a word below is a black/blocked cell, derived
/// at runtime rather than stored here. See CrosswordPuzzleData.cs for the shared types.
/// </summary>
public static class Cebuano_Level4
{
    public static readonly CrosswordPuzzle Data = new CrosswordPuzzle("Cebuano", 4, new List<CrosswordWordEntry>
    {
        new CrosswordWordEntry(1, "ILBU", "This is the bent pipe fitting used to change the direction of a pipeline.", 0, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(2, "UDTO", "This is the middle of the day, exactly twelve o'clock.", 0, 3, CrosswordDirection.Down),
        new CrosswordWordEntry(3, "ITUT", "This is the intimate physical act shared between two people in a relationship.", 2, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(3, "IMAS", "This word means something extra has been included or added.", 2, 0, CrosswordDirection.Down),
        new CrosswordWordEntry(4, "AGULU", "This is the low sound someone makes when in pain or discomfort.", 4, 2, CrosswordDirection.Down),
        new CrosswordWordEntry(5, "SUGOD", "This is the very start of something.", 5, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(6, "DIP", "This is the dance move where one partner leans the other backward dramatically.", 5, 4, CrosswordDirection.Down),
        new CrosswordWordEntry(7, "HULUP", "This means to land or come to rest on something, like a bird settling on a branch.", 7, 0, CrosswordDirection.Across),
    });
}
