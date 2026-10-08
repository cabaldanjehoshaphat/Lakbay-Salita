using System.Collections.Generic;

/// <summary>
/// Cebuano_Level8
/// Hardcoded crossword puzzle data for Cebuano level 8 (6 across,
/// 6 down; 4-8 letter words). Grid position is 0-indexed (row 0 = top,
/// col 0 = left); any cell not covered by a word below is a black/blocked cell, derived
/// at runtime rather than stored here. See CrosswordPuzzleData.cs for the shared types.
/// </summary>
public static class Cebuano_Level8
{
    public static readonly CrosswordPuzzle Data = new CrosswordPuzzle("Cebuano", 8, new List<CrosswordWordEntry>
    {
        new CrosswordWordEntry(1, "NAPU", "This describes level ground that is rich and good for growing crops.", 0, 1, CrosswordDirection.Down),
        new CrosswordWordEntry(2, "BAKAK", "This describes a statement that is not true.", 0, 6, CrosswordDirection.Down),
        new CrosswordWordEntry(3, "AGUD", "This phrase is used to state the purpose or reason for doing something.", 0, 11, CrosswordDirection.Across),
        new CrosswordWordEntry(4, "UNUM", "This is the number that comes after five.", 0, 13, CrosswordDirection.Down),
        new CrosswordWordEntry(5, "MAGBAYAD", "This means to give money in exchange for goods or services.", 1, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(6, "APUD", "This means to distribute a share to every person.", 1, 4, CrosswordDirection.Down),
        new CrosswordWordEntry(7, "TITI", "This is the male reproductive organ.", 1, 10, CrosswordDirection.Down),
        new CrosswordWordEntry(8, "ATUM", "This is the smallest basic unit of a chemical element.", 1, 15, CrosswordDirection.Down),
        new CrosswordWordEntry(9, "DIDTU", "This word points to a place that is far from the speaker.", 2, 9, CrosswordDirection.Across),
        new CrosswordWordEntry(10, "TUNTU", "This describes someone who acts in a foolish or unwise way.", 3, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(11, "MAUNG", "This is the sturdy blue cotton fabric used to make jeans.", 3, 13, CrosswordDirection.Across),
        new CrosswordWordEntry(12, "DIKLIMIR", "This is a person who recites something aloud dramatically, as in a speech.", 4, 4, CrosswordDirection.Across),
    });
}
