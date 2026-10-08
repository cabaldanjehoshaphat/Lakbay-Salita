using System.Collections.Generic;

/// <summary>
/// Hiligaynon_Level7
/// Hardcoded crossword puzzle data for Hiligaynon level 7 (6 across,
/// 5 down; 4-8 letter words). Grid position is 0-indexed (row 0 = top,
/// col 0 = left); any cell not covered by a word below is a black/blocked cell, derived
/// at runtime rather than stored here. See CrosswordPuzzleData.cs for the shared types.
/// </summary>
public static class Hiligaynon_Level7
{
    public static readonly CrosswordPuzzle Data = new CrosswordPuzzle("Hiligaynon", 7, new List<CrosswordWordEntry>
    {
        new CrosswordWordEntry(1, "UYSIT", "This describes someone who is experiencing good fortune and joy.", 0, 3, CrosswordDirection.Across),
        new CrosswordWordEntry(2, "SIDO", "This is the sudden, involuntary spasm that makes a person hiccup.", 0, 5, CrosswordDirection.Down),
        new CrosswordWordEntry(3, "KARGADOR", "This is a person hired to carry heavy loads or luggage.", 2, 3, CrosswordDirection.Down),
        new CrosswordWordEntry(4, "UTUD", "This is the general term for a sibling.", 2, 8, CrosswordDirection.Down),
        new CrosswordWordEntry(5, "PAYOPOT", "This is a small round projection or fastener sticking out from a surface.", 3, 2, CrosswordDirection.Across),
        new CrosswordWordEntry(6, "PANGGOI", "This means to go out and gather seafood such as fish or crabs.", 3, 6, CrosswordDirection.Down),
        new CrosswordWordEntry(7, "DARA", "This is the sister of one's mother or father.", 5, 8, CrosswordDirection.Across),
        new CrosswordWordEntry(8, "PAHABUG", "This means to throw something with great force.", 6, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(9, "APAS", "This means to pursue or chase after someone or something.", 6, 1, CrosswordDirection.Down),
        new CrosswordWordEntry(10, "WALO", "This is the number that comes after seven.", 8, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(11, "GOOD", "This is an empty space or opening within something solid.", 8, 5, CrosswordDirection.Across),
    });
}
