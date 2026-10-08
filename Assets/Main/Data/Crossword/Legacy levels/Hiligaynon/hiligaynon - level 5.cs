using System.Collections.Generic;

/// <summary>
/// Hiligaynon_Level5
/// Hardcoded crossword puzzle data for Hiligaynon level 5 (5 across,
/// 4 down; 4-7 letter words). Grid position is 0-indexed (row 0 = top,
/// col 0 = left); any cell not covered by a word below is a black/blocked cell, derived
/// at runtime rather than stored here. See CrosswordPuzzleData.cs for the shared types.
/// </summary>
public static class Hiligaynon_Level5
{
    public static readonly CrosswordPuzzle Data = new CrosswordPuzzle("Hiligaynon", 5, new List<CrosswordWordEntry>
    {
        new CrosswordWordEntry(1, "ATOP", "This is the covering that protects the top of a building.", 0, 4, CrosswordDirection.Across),
        new CrosswordWordEntry(2, "TUTU", "This means to strike something repeatedly and heavily, like grinding rice.", 0, 5, CrosswordDirection.Down),
        new CrosswordWordEntry(3, "PAMADO", "This describes something built to withstand wear for a long time.", 2, 3, CrosswordDirection.Down),
        new CrosswordWordEntry(4, "PAPULA", "This means to cause something to turn red in color.", 3, 2, CrosswordDirection.Across),
        new CrosswordWordEntry(5, "LAMA", "This means to smash something into pieces with force.", 3, 6, CrosswordDirection.Down),
        new CrosswordWordEntry(6, "DOLA", "This means to no longer have something, often by misplacing it.", 6, 3, CrosswordDirection.Across),
        new CrosswordWordEntry(7, "LUMA", "This describes something that is aged and no longer useful.", 6, 5, CrosswordDirection.Down),
        new CrosswordWordEntry(8, "POKO", "This is a serious sexually transmitted infection.", 7, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(9, "YABI", "This is the metal tool used to lock or unlock a door.", 9, 4, CrosswordDirection.Across),
    });
}
