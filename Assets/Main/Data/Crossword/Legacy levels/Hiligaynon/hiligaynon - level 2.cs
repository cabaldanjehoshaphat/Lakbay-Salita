using System.Collections.Generic;

/// <summary>
/// Hiligaynon_Level2
/// Hardcoded crossword puzzle data for Hiligaynon level 2 (3 across,
/// 3 down; 3-5 letter words). Grid position is 0-indexed (row 0 = top,
/// col 0 = left); any cell not covered by a word below is a black/blocked cell, derived
/// at runtime rather than stored here. See CrosswordPuzzleData.cs for the shared types.
/// </summary>
public static class Hiligaynon_Level2
{
    public static readonly CrosswordPuzzle Data = new CrosswordPuzzle("Hiligaynon", 2, new List<CrosswordWordEntry>
    {
        new CrosswordWordEntry(1, "WALA", "This refers to the side opposite the right, or the hand on that side.", 0, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(2, "ANO", "This is the opening at the end of the digestive tract.", 0, 1, CrosswordDirection.Down),
        new CrosswordWordEntry(3, "POLO", "This is a piece of land completely surrounded by water.", 0, 6, CrosswordDirection.Down),
        new CrosswordWordEntry(4, "ASO", "This is the visible gas released when something burns.", 1, 4, CrosswordDirection.Across),
        new CrosswordWordEntry(4, "ANI", "This is the season or act of gathering ripe rice from the fields.", 1, 4, CrosswordDirection.Down),
        new CrosswordWordEntry(5, "MOTON", "This is a wheel-and-rope device used to lift heavy objects.", 2, 0, CrosswordDirection.Across),
    });
}
