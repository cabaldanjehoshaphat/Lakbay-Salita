using System.Collections.Generic;

/// <summary>
/// Tagalog_Level6
/// Hardcoded crossword puzzle data for Tagalog level 6 (5 across,
/// 5 down; 4-7 letter words). Grid position is 0-indexed (row 0 = top,
/// col 0 = left); any cell not covered by a word below is a black/blocked cell, derived
/// at runtime rather than stored here. See CrosswordPuzzleData.cs for the shared types.
/// </summary>
public static class Tagalog_Level6
{
    public static readonly CrosswordPuzzle Data = new CrosswordPuzzle("Tagalog", 6, new List<CrosswordWordEntry>
    {
        new CrosswordWordEntry(1, "SANDO", "This is a simple sleeveless shirt usually worn under other clothing.", 0, 0, CrosswordDirection.Down),
        new CrosswordWordEntry(2, "KARIL", "This is a groove left in the ground by the repeated passing of wheels.", 0, 5, CrosswordDirection.Down),
        new CrosswordWordEntry(3, "NAIK", "This is a residential area located on the outskirts of a city.", 0, 10, CrosswordDirection.Across),
        new CrosswordWordEntry(4, "IBIDA", "This means to tell a story or event again in detail.", 0, 12, CrosswordDirection.Down),
        new CrosswordWordEntry(5, "MAIL", "This is letters and packages sent through a postal service.", 1, 4, CrosswordDirection.Across),
        new CrosswordWordEntry(6, "LUPI", "This means to bend something over onto itself, like paper or cloth.", 1, 7, CrosswordDirection.Down),
        new CrosswordWordEntry(7, "PAKO", "This is a small metal spike used to join pieces of wood together.", 1, 9, CrosswordDirection.Down),
        new CrosswordWordEntry(8, "ANGI", "This is the distinct smell given off when rice is scorched while cooking.", 2, 9, CrosswordDirection.Across),
        new CrosswordWordEntry(9, "DALIRI", "This is one of the five digits at the end of the hand.", 3, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(10, "IMORTAL", "This describes someone or something that lives forever and never dies.", 4, 7, CrosswordDirection.Across),
    });
}
