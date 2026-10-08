using System.Collections.Generic;

/// <summary>
/// Cebuano_Level2
/// Hardcoded crossword puzzle data for Cebuano level 2 (3 across,
/// 3 down; 3-5 letter words). Grid position is 0-indexed (row 0 = top,
/// col 0 = left); any cell not covered by a word below is a black/blocked cell, derived
/// at runtime rather than stored here. See CrosswordPuzzleData.cs for the shared types.
/// </summary>
public static class Cebuano_Level2
{
    public static readonly CrosswordPuzzle Data = new CrosswordPuzzle("Cebuano", 2, new List<CrosswordWordEntry>
    {
        new CrosswordWordEntry(1, "TUNO", "This is the creamy liquid squeezed from grated coconut meat.", 0, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(2, "NAKO", "This word is used to show that something belongs to the speaker.", 0, 2, CrosswordDirection.Down),
        new CrosswordWordEntry(3, "IMU", "This pronoun is used to refer to the person being spoken to.", 1, 4, CrosswordDirection.Down),
        new CrosswordWordEntry(4, "RID", "This is a sudden, surprise operation carried out by police to catch offenders.", 1, 6, CrosswordDirection.Down),
        new CrosswordWordEntry(5, "KAMPI", "This means to support or favor one side in a dispute.", 2, 2, CrosswordDirection.Across),
        new CrosswordWordEntry(6, "UNO", "This is the number that comes right after zero.", 3, 0, CrosswordDirection.Across),
    });
}
