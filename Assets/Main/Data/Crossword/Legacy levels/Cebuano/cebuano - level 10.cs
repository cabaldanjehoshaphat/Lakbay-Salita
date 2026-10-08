using System.Collections.Generic;

/// <summary>
/// Cebuano_Level10
/// Hardcoded crossword puzzle data for Cebuano level 10 (7 across,
/// 7 down; 5-8 letter words). Grid position is 0-indexed (row 0 = top,
/// col 0 = left); any cell not covered by a word below is a black/blocked cell, derived
/// at runtime rather than stored here. See CrosswordPuzzleData.cs for the shared types.
/// </summary>
public static class Cebuano_Level10
{
    public static readonly CrosswordPuzzle Data = new CrosswordPuzzle("Cebuano", 10, new List<CrosswordWordEntry>
    {
        new CrosswordWordEntry(1, "LAGDU", "This refers to tiny drops of liquid, such as water or rain.", 0, 3, CrosswordDirection.Down),
        new CrosswordWordEntry(2, "PATAK", "This is an irregular patch or spot of color or dirt on a surface.", 0, 6, CrosswordDirection.Down),
        new CrosswordWordEntry(3, "KATAP", "This means to cover an area completely and evenly.", 0, 8, CrosswordDirection.Down),
        new CrosswordWordEntry(4, "ATSARA", "This is a dish of vegetables preserved in vinegar and spices.", 1, 3, CrosswordDirection.Across),
        new CrosswordWordEntry(5, "GUSOK", "This is one of the curved bones that form the chest cage.", 4, 2, CrosswordDirection.Across),
        new CrosswordWordEntry(6, "SUGBA", "This means to cook food directly over glowing embers or coals.", 4, 4, CrosswordDirection.Down),
        new CrosswordWordEntry(7, "BUGTO", "This means to snap or separate something into pieces.", 6, 2, CrosswordDirection.Across),
        new CrosswordWordEntry(8, "KUYAMANG", "This describes the way small insects move slowly along a surface.", 7, 1, CrosswordDirection.Down),
        new CrosswordWordEntry(9, "BUNTAS", "This describes someone who is extremely hungry from lack of food.", 8, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(10, "UKBAN", "This is a type of citrus fruit.", 9, 6, CrosswordDirection.Down),
        new CrosswordWordEntry(11, "KASUPAK", "This is the person or side one competes against in a contest.", 10, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(12, "PILYA", "This describes someone, often a child, who enjoys playful mischief.", 10, 4, CrosswordDirection.Down),
        new CrosswordWordEntry(13, "KAKALMA", "This is a state of being calm and peaceful.", 12, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(14, "GIKAN", "This word indicates the origin or starting point of something.", 14, 1, CrosswordDirection.Across),
    });
}
