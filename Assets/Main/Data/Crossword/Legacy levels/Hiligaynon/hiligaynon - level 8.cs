using System.Collections.Generic;

/// <summary>
/// Hiligaynon_Level8
/// Hardcoded crossword puzzle data for Hiligaynon level 8 (6 across,
/// 6 down; 4-8 letter words). Grid position is 0-indexed (row 0 = top,
/// col 0 = left); any cell not covered by a word below is a black/blocked cell, derived
/// at runtime rather than stored here. See CrosswordPuzzleData.cs for the shared types.
/// </summary>
public static class Hiligaynon_Level8
{
    public static readonly CrosswordPuzzle Data = new CrosswordPuzzle("Hiligaynon", 8, new List<CrosswordWordEntry>
    {
        new CrosswordWordEntry(1, "PALUKPUK", "This is the hard central core of an ear of corn, left after the kernels are removed.", 0, 5, CrosswordDirection.Down),
        new CrosswordWordEntry(2, "SAHIN", "This is the small part left over after most of something has been used.", 0, 7, CrosswordDirection.Down),
        new CrosswordWordEntry(3, "PARAS", "This is one of the small bones that make up the spine.", 1, 4, CrosswordDirection.Across),
        new CrosswordWordEntry(4, "PUKING", "This is the external part of the female genitalia.", 4, 3, CrosswordDirection.Across),
        new CrosswordWordEntry(4, "PUYO", "This is a type of fish found in local waters.", 4, 3, CrosswordDirection.Down),
        new CrosswordWordEntry(5, "GAHI", "This is the money someone earns for their work.", 4, 8, CrosswordDirection.Down),
        new CrosswordWordEntry(6, "HOSO", "This means to pull a weapon, like a sword, out of its case.", 6, 8, CrosswordDirection.Across),
        new CrosswordWordEntry(7, "DURO", "This word describes a large number of something.", 7, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(8, "KAGI", "This means to violently rip something apart.", 7, 5, CrosswordDirection.Across),
        new CrosswordWordEntry(9, "ALWAK", "This refers to liquid that has accidentally poured out of its container.", 7, 6, CrosswordDirection.Down),
        new CrosswordWordEntry(10, "DAIT", "This is a state of calm with no conflict or war.", 8, 4, CrosswordDirection.Down),
        new CrosswordWordEntry(11, "KILAB", "This is a brief, sudden burst of light.", 10, 3, CrosswordDirection.Across),
    });
}
