using System.Collections.Generic;

/// <summary>
/// Tagalog_Level2
/// Hardcoded crossword puzzle data for Tagalog level 2 (3 across,
/// 3 down; 3-5 letter words). Grid position is 0-indexed (row 0 = top,
/// col 0 = left); any cell not covered by a word below is a black/blocked cell, derived
/// at runtime rather than stored here. See CrosswordPuzzleData.cs for the shared types.
/// </summary>
public static class Tagalog_Level2
{
    public static readonly CrosswordPuzzle Data = new CrosswordPuzzle("Tagalog", 2, new List<CrosswordWordEntry>
    {
        new CrosswordWordEntry(1, "BAKSA", "This is a cloth draped over the shoulder as an accessory.", 0, 3, CrosswordDirection.Down),
        new CrosswordWordEntry(2, "LIPAY", "This is a rough-textured wild plant used in traditional remedies.", 0, 6, CrosswordDirection.Down),
        new CrosswordWordEntry(3, "KATAL", "This describes shaking, usually from fear, cold, or weakness.", 1, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(4, "AGA", "This describes arriving or happening before the expected time.", 1, 1, CrosswordDirection.Down),
        new CrosswordWordEntry(5, "TAAS", "This is the measurement of how tall something is.", 3, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(6, "ARAY", "This is the exclamation people shout when they suddenly feel pain.", 4, 3, CrosswordDirection.Across),
    });
}
