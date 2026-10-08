using System.Collections.Generic;

/// <summary>
/// Hiligaynon_Level6
/// Hardcoded crossword puzzle data for Hiligaynon level 6 (5 across,
/// 5 down; 4-7 letter words). Grid position is 0-indexed (row 0 = top,
/// col 0 = left); any cell not covered by a word below is a black/blocked cell, derived
/// at runtime rather than stored here. See CrosswordPuzzleData.cs for the shared types.
/// </summary>
public static class Hiligaynon_Level6
{
    public static readonly CrosswordPuzzle Data = new CrosswordPuzzle("Hiligaynon", 6, new List<CrosswordWordEntry>
    {
        new CrosswordWordEntry(1, "BELA", "This is a stick of wax with a wick, burned to give light.", 0, 1, CrosswordDirection.Down),
        new CrosswordWordEntry(2, "APIKE", "This phrase means something is about to happen very soon.", 3, 1, CrosswordDirection.Across),
        new CrosswordWordEntry(3, "KIBO", "This is a steady, rhythmic pulsing feeling or sound.", 3, 4, CrosswordDirection.Down),
        new CrosswordWordEntry(4, "DAAY", "This is a small, compact chunk of soil.", 5, 1, CrosswordDirection.Down),
        new CrosswordWordEntry(5, "TAKMO", "This means to draw liquid or air into the mouth by creating suction.", 6, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(6, "NYEBE", "This is the soft, white frozen precipitation that falls in cold climates.", 8, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(7, "BADAHO", "This is the small piece inside a bell that strikes its side to make sound.", 8, 3, CrosswordDirection.Down),
        new CrosswordWordEntry(8, "KADA", "This word refers to every single one in a group, taken individually.", 10, 1, CrosswordDirection.Down),
        new CrosswordWordEntry(9, "WAWAW", "This is a loud, sharp cry, like the sound a dog or wolf makes.", 11, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(10, "NANO", "This question word is used to ask for the identity or nature of something.", 13, 0, CrosswordDirection.Across),
    });
}
