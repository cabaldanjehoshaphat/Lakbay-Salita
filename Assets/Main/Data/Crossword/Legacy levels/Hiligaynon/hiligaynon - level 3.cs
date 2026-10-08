using System.Collections.Generic;

/// <summary>
/// Hiligaynon_Level3
/// Hardcoded crossword puzzle data for Hiligaynon level 3 (4 across,
/// 3 down; 3-6 letter words). Grid position is 0-indexed (row 0 = top,
/// col 0 = left); any cell not covered by a word below is a black/blocked cell, derived
/// at runtime rather than stored here. See CrosswordPuzzleData.cs for the shared types.
/// </summary>
public static class Hiligaynon_Level3
{
    public static readonly CrosswordPuzzle Data = new CrosswordPuzzle("Hiligaynon", 3, new List<CrosswordWordEntry>
    {
        new CrosswordWordEntry(1, "LAGSAW", "This is a graceful, hoofed animal known for its antlers.", 0, 0, CrosswordDirection.Down),
        new CrosswordWordEntry(2, "ARMAS", "This refers to tools or instruments used for fighting or self-defense.", 1, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(3, "SUA", "This is a citrus tree that produces orange-like fruit.", 1, 4, CrosswordDirection.Down),
        new CrosswordWordEntry(4, "SUKLA", "This is a smooth, shiny fabric made from the fibers spun by silkworms.", 3, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(5, "KALAYA", "This describes the state of having no moisture at all.", 3, 2, CrosswordDirection.Down),
        new CrosswordWordEntry(6, "WALI", "This is a talk given to teach a moral or religious lesson.", 5, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(7, "BATA", "This is a young human being who is not yet an adult.", 8, 1, CrosswordDirection.Across),
    });
}
