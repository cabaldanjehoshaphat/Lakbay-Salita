using System.Collections.Generic;

/// <summary>
/// Tagalog_Level4
/// Hardcoded crossword puzzle data for Tagalog level 4 (4 across,
/// 4 down; 3-6 letter words). Grid position is 0-indexed (row 0 = top,
/// col 0 = left); any cell not covered by a word below is a black/blocked cell, derived
/// at runtime rather than stored here. See CrosswordPuzzleData.cs for the shared types.
/// </summary>
public static class Tagalog_Level4
{
    public static readonly CrosswordPuzzle Data = new CrosswordPuzzle("Tagalog", 4, new List<CrosswordWordEntry>
    {
        new CrosswordWordEntry(1, "BONETE", "This is a soft hat tied under the chin, often worn by babies or in older fashion.", 0, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(2, "EHE", "This is the rod that connects and turns a pair of wheels.", 0, 5, CrosswordDirection.Down),
        new CrosswordWordEntry(3, "PULUBE", "This is a person who is extremely poor.", 2, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(4, "UMIKIT", "This means to spin around quickly and lightly.", 2, 1, CrosswordDirection.Down),
        new CrosswordWordEntry(5, "BIRO", "This is something said or done to make people laugh.", 2, 4, CrosswordDirection.Down),
        new CrosswordWordEntry(6, "KUTO", "This describes a large group of insects moving together.", 5, 1, CrosswordDirection.Across),
        new CrosswordWordEntry(7, "TIYONG", "This is the brother of one's mother or father.", 5, 3, CrosswordDirection.Down),
        new CrosswordWordEntry(8, "TAYO", "This pronoun refers to the speaker and at least one other person, including the listener.", 7, 1, CrosswordDirection.Across),
    });
}
