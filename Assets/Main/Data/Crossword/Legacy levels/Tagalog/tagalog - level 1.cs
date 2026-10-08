using System.Collections.Generic;

/// <summary>
/// Tagalog_Level1
/// Hardcoded crossword puzzle data for Tagalog level 1 (3 across,
/// 2 down; 3-5 letter words). Grid position is 0-indexed (row 0 = top,
/// col 0 = left); any cell not covered by a word below is a black/blocked cell, derived
/// at runtime rather than stored here. See CrosswordPuzzleData.cs for the shared types.
/// </summary>
public static class Tagalog_Level1
{
    public static readonly CrosswordPuzzle Data = new CrosswordPuzzle("Tagalog", 1, new List<CrosswordWordEntry>
    {
        new CrosswordWordEntry(1, "UMAY", "This describes the sick feeling of having had too much of something, especially food.", 0, 4, CrosswordDirection.Across),
        new CrosswordWordEntry(1, "UBAS", "This is a small, round, juicy fruit that grows in clusters on a vine.", 0, 4, CrosswordDirection.Down),
        new CrosswordWordEntry(2, "AMO", "This describes an animal's gentle, calm behavior after being trained.", 0, 6, CrosswordDirection.Down),
        new CrosswordWordEntry(3, "ALO", "This is anything that comforts or lifts someone's spirits.", 2, 4, CrosswordDirection.Across),
        new CrosswordWordEntry(4, "PILUS", "This is a soft fabric with a dense, short pile on one side.", 3, 0, CrosswordDirection.Across),
    });
}
