using System.Collections.Generic;

/// <summary>
/// Tagalog_Level9
/// Hardcoded crossword puzzle data for Tagalog level 9 (7 across,
/// 6 down; 5-8 letter words). Grid position is 0-indexed (row 0 = top,
/// col 0 = left); any cell not covered by a word below is a black/blocked cell, derived
/// at runtime rather than stored here. See CrosswordPuzzleData.cs for the shared types.
/// </summary>
public static class Tagalog_Level9
{
    public static readonly CrosswordPuzzle Data = new CrosswordPuzzle("Tagalog", 9, new List<CrosswordWordEntry>
    {
        new CrosswordWordEntry(1, "DASTO", "This is a small remaining trace of something that once existed.", 0, 4, CrosswordDirection.Down),
        new CrosswordWordEntry(2, "MOLDURA", "This is a decorative strip used to frame or trim edges, like around a picture.", 0, 7, CrosswordDirection.Down),
        new CrosswordWordEntry(3, "MAPOOT", "This means to hate or detest something intensely.", 1, 3, CrosswordDirection.Across),
        new CrosswordWordEntry(4, "BALUBALO", "This describes acting as if something is true when it is not, for fun or deception.", 3, 2, CrosswordDirection.Down),
        new CrosswordWordEntry(5, "TINDI", "This is the strength or degree of force behind something.", 3, 4, CrosswordDirection.Across),
        new CrosswordWordEntry(6, "UNANO", "This is a person or creature of unusually small stature.", 4, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(7, "BULANYA", "This is the crime of taking someone else's property without permission.", 6, 1, CrosswordDirection.Across),
        new CrosswordWordEntry(8, "AGUHA", "This is a thin, pointed tool used for sewing.", 8, 5, CrosswordDirection.Down),
        new CrosswordWordEntry(9, "ILUNGAN", "This describes someone with an unusually large nose.", 9, 1, CrosswordDirection.Across),
        new CrosswordWordEntry(10, "YAPAK", "This means to place one's foot down on top of an object or surface.", 10, 0, CrosswordDirection.Down),
        new CrosswordWordEntry(11, "BINYAGAN", "This means to perform the religious ritual of blessing someone with water.", 11, 3, CrosswordDirection.Down),
        new CrosswordWordEntry(12, "PASIGAW", "This describes speaking or shouting in a voice loud enough for others to hear.", 12, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(13, "KANYA", "This word is used to introduce a result or conclusion.", 14, 0, CrosswordDirection.Across),
    });
}
