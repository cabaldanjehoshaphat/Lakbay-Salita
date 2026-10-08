using System.Collections.Generic;

/// <summary>
/// Tagalog_Level7
/// Hardcoded crossword puzzle data for Tagalog level 7 (6 across,
/// 5 down; 4-8 letter words). Grid position is 0-indexed (row 0 = top,
/// col 0 = left); any cell not covered by a word below is a black/blocked cell, derived
/// at runtime rather than stored here. See CrosswordPuzzleData.cs for the shared types.
/// </summary>
public static class Tagalog_Level7
{
    public static readonly CrosswordPuzzle Data = new CrosswordPuzzle("Tagalog", 7, new List<CrosswordWordEntry>
    {
        new CrosswordWordEntry(1, "DAGASA", "This describes someone who acts suddenly without thinking things through.", 0, 3, CrosswordDirection.Across),
        new CrosswordWordEntry(2, "SERB", "This means to perform duties for someone, or to present food to them.", 0, 7, CrosswordDirection.Down),
        new CrosswordWordEntry(3, "MAYURYA", "This is the larger part of a group, more than half.", 2, 3, CrosswordDirection.Across),
        new CrosswordWordEntry(4, "APAW", "This describes a container so full that its contents spill over the edge.", 2, 9, CrosswordDirection.Down),
        new CrosswordWordEntry(5, "GUGUIN", "This means to use a substance to wash and clean the hair.", 4, 4, CrosswordDirection.Down),
        new CrosswordWordEntry(6, "LUMITAW", "This means to come into view or become visible.", 5, 3, CrosswordDirection.Across),
        new CrosswordWordEntry(7, "ISLA", "This is a piece of land completely surrounded by water.", 5, 6, CrosswordDirection.Down),
        new CrosswordWordEntry(8, "AKYATAN", "This means to carry an item up to a higher floor.", 5, 8, CrosswordDirection.Down),
        new CrosswordWordEntry(9, "IGAPAS", "This means to use a tool to cut down grass or crops.", 8, 4, CrosswordDirection.Across),
        new CrosswordWordEntry(10, "LUWAN", "This is a type of edible marine algae.", 9, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(11, "KUTA", "This is a strong, defensive structure built to protect against attack.", 10, 5, CrosswordDirection.Across),
    });
}
