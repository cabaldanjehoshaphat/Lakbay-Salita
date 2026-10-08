using System.Collections.Generic;

/// <summary>
/// Hiligaynon_Level10
/// Hardcoded crossword puzzle data for Hiligaynon level 10 (7 across,
/// 7 down; 5-8 letter words). Grid position is 0-indexed (row 0 = top,
/// col 0 = left); any cell not covered by a word below is a black/blocked cell, derived
/// at runtime rather than stored here. See CrosswordPuzzleData.cs for the shared types.
/// </summary>
public static class Hiligaynon_Level10
{
    public static readonly CrosswordPuzzle Data = new CrosswordPuzzle("Hiligaynon", 10, new List<CrosswordWordEntry>
    {
        new CrosswordWordEntry(1, "SUNGAB", "This is a small cut or dent carved into a surface.", 0, 11, CrosswordDirection.Across),
        new CrosswordWordEntry(2, "BULAN", "This is the natural satellite that orbits the Earth, or a unit of time based on its cycle.", 0, 16, CrosswordDirection.Down),
        new CrosswordWordEntry(3, "RARIM", "This is the sense used to perceive the flavor of food.", 2, 2, CrosswordDirection.Down),
        new CrosswordWordEntry(4, "ILAYA", "This describes a direction facing toward the land, away from the sea.", 2, 5, CrosswordDirection.Down),
        new CrosswordWordEntry(5, "DAMGO", "This is the series of images and thoughts that occur in the mind during sleep.", 2, 8, CrosswordDirection.Down),
        new CrosswordWordEntry(6, "METAL", "This is a hard, shiny material like iron or gold, often used to make tools.", 2, 10, CrosswordDirection.Across),
        new CrosswordWordEntry(6, "MAYAD", "This describes something done properly or in a satisfactory way.", 2, 10, CrosswordDirection.Down),
        new CrosswordWordEntry(7, "LAHAT", "This word refers to the complete amount or entirety of something.", 2, 14, CrosswordDirection.Down),
        new CrosswordWordEntry(8, "FRANELA", "This is a soft, warm fabric often used for winter clothing.", 3, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(9, "OHANG", "This describes something with great size in width or depth.", 4, 13, CrosswordDirection.Across),
        new CrosswordWordEntry(10, "YANGHAG", "This describes looking at something with a fixed, wide-eyed gaze.", 5, 5, CrosswordDirection.Across),
        new CrosswordWordEntry(11, "KAMISA", "This is a piece of clothing worn on the upper body.", 6, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(12, "SATIN", "This is a smooth, glossy cotton fabric that resembles satin.", 6, 4, CrosswordDirection.Down),
        new CrosswordWordEntry(13, "KATRE", "This is the furniture piece used for sleeping or resting.", 6, 12, CrosswordDirection.Across),
    });
}
