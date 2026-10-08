using System.Collections.Generic;

/// <summary>
/// Cebuano_Level1
/// Hardcoded crossword puzzle data for Cebuano level 1 (3 across,
/// 2 down; 3-5 letter words). Grid position is 0-indexed (row 0 = top,
/// col 0 = left); any cell not covered by a word below is a black/blocked cell, derived
/// at runtime rather than stored here. See CrosswordPuzzleData.cs for the shared types.
/// </summary>
public static class Cebuano_Level1
{
    public static readonly CrosswordPuzzle Data = new CrosswordPuzzle("Cebuano", 1, new List<CrosswordWordEntry>
    {
        new CrosswordWordEntry(1, "BANGS", "This is hair that is cut short and combed to fall over the forehead.", 0, 3, CrosswordDirection.Across),
        new CrosswordWordEntry(1, "BAT", "This is the wooden club a batter swings to hit the ball in baseball.", 0, 3, CrosswordDirection.Down),
        new CrosswordWordEntry(2, "GISU", "This is a small, round, green citrus fruit with very sour juice.", 0, 6, CrosswordDirection.Down),
        new CrosswordWordEntry(3, "GATA", "This is the large tropical fruit whose milk and flesh are used in many Filipino dishes.", 1, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(4, "TUKSO", "This is a short test given to check what a student has learned.", 2, 3, CrosswordDirection.Across),
    });
}
