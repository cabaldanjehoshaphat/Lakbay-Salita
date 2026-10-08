using System.Collections.Generic;

/// <summary>
/// Tagalog_Level10
/// Hardcoded crossword puzzle data for Tagalog level 10 (7 across,
/// 7 down; 5-8 letter words). Grid position is 0-indexed (row 0 = top,
/// col 0 = left); any cell not covered by a word below is a black/blocked cell, derived
/// at runtime rather than stored here. See CrosswordPuzzleData.cs for the shared types.
/// </summary>
public static class Tagalog_Level10
{
    public static readonly CrosswordPuzzle Data = new CrosswordPuzzle("Tagalog", 10, new List<CrosswordWordEntry>
    {
        new CrosswordWordEntry(1, "BUKAL", "This describes something that exists in nature, not made or caused by people.", 0, 1, CrosswordDirection.Down),
        new CrosswordWordEntry(2, "IDYOMA", "This is a system of words and grammar used by a group of people to communicate.", 0, 5, CrosswordDirection.Down),
        new CrosswordWordEntry(3, "MALIMALI", "This describes work or writing that contains many errors.", 2, 3, CrosswordDirection.Down),
        new CrosswordWordEntry(4, "PAYAMOT", "This describes acting or reacting in an easily annoyed, impatient way.", 3, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(5, "PIRAT", "This describes something that has been pressed down or crushed flat.", 5, 2, CrosswordDirection.Across),
        new CrosswordWordEntry(6, "MAKALALA", "This means to make a situation or someone's mood worse.", 6, 1, CrosswordDirection.Down),
        new CrosswordWordEntry(7, "KALABIT", "This is the act of plucking or brushing across the strings of an instrument.", 7, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(8, "TAMPA", "This is an offer of a price, especially at an auction.", 7, 6, CrosswordDirection.Down),
        new CrosswordWordEntry(9, "PAKISAMA", "This refers to a community of people living together under shared customs.", 9, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(10, "MANYIKA", "This is a toy made to look like a small human figure.", 11, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(11, "IANAK", "This means to bring a baby into the world.", 11, 4, CrosswordDirection.Down),
        new CrosswordWordEntry(12, "MAPANOOD", "This describes having the opportunity or ability to view a performance or program.", 13, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(13, "PASOK", "This is the act of going into a place, or the point where one enters.", 13, 2, CrosswordDirection.Down),
        new CrosswordWordEntry(14, "HASIK", "This is the act of scattering seeds over soil to grow crops.", 15, 0, CrosswordDirection.Across),
    });
}
