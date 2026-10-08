using System.Collections.Generic;

/// <summary>
/// Cebuano_Level3
/// Hardcoded crossword puzzle data for Cebuano level 3 (4 across,
/// 3 down; 3-6 letter words). Grid position is 0-indexed (row 0 = top,
/// col 0 = left); any cell not covered by a word below is a black/blocked cell, derived
/// at runtime rather than stored here. See CrosswordPuzzleData.cs for the shared types.
/// </summary>
public static class Cebuano_Level3
{
    public static readonly CrosswordPuzzle Data = new CrosswordPuzzle("Cebuano", 3, new List<CrosswordWordEntry>
    {
        new CrosswordWordEntry(1, "BIKUG", "This means to shift or loosen something that has been fixed in place.", 0, 6, CrosswordDirection.Down),
        new CrosswordWordEntry(2, "SAB", "This is the time of day between afternoon and nightfall.", 1, 1, CrosswordDirection.Down),
        new CrosswordWordEntry(3, "GISI", "This means to tear or rip something roughly, leaving a jagged wound.", 1, 3, CrosswordDirection.Down),
        new CrosswordWordEntry(4, "RIN", "This word is used to add another idea to what was just said.", 1, 5, CrosswordDirection.Across),
        new CrosswordWordEntry(5, "PANIG", "This refers to one of the surfaces bordering something, or a party in a dispute.", 2, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(6, "UNUSON", "This describes wind that blows in strong, sudden bursts.", 3, 6, CrosswordDirection.Across),
        new CrosswordWordEntry(7, "IBOG", "This is the feeling of wanting what someone else has.", 4, 3, CrosswordDirection.Across),
    });
}
