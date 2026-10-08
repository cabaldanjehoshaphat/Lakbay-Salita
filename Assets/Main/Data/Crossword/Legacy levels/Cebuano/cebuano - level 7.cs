using System.Collections.Generic;

/// <summary>
/// Cebuano_Level7
/// Hardcoded crossword puzzle data for Cebuano level 7 (6 across,
/// 5 down; 4-8 letter words). Grid position is 0-indexed (row 0 = top,
/// col 0 = left); any cell not covered by a word below is a black/blocked cell, derived
/// at runtime rather than stored here. See CrosswordPuzzleData.cs for the shared types.
/// </summary>
public static class Cebuano_Level7
{
    public static readonly CrosswordPuzzle Data = new CrosswordPuzzle("Cebuano", 7, new List<CrosswordWordEntry>
    {
        new CrosswordWordEntry(1, "UBAS", "This is a small, round, juicy fruit that grows in clusters on a vine.", 0, 1, CrosswordDirection.Across),
        new CrosswordWordEntry(2, "BULILYUS", "This is the bottle-shaped target that bowlers try to knock down.", 0, 2, CrosswordDirection.Down),
        new CrosswordWordEntry(3, "RAUK", "This is the currency used to buy and sell goods.", 1, 0, CrosswordDirection.Down),
        new CrosswordWordEntry(4, "ASLUM", "This describes a sharp, tangy taste like that of a lemon.", 2, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(5, "MAMILINO", "This is the person who drives a horse-drawn carriage.", 2, 4, CrosswordDirection.Down),
        new CrosswordWordEntry(6, "PATA", "This describes something dropping and landing with a soft, heavy sound.", 3, 6, CrosswordDirection.Across),
        new CrosswordWordEntry(6, "PRUNA", "This is a dried plum.", 3, 6, CrosswordDirection.Down),
        new CrosswordWordEntry(7, "KILIM", "This is the name for a certain large, tall tree species.", 4, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(8, "DIAY", "This word is used to express agreement or confirm a mild surprise.", 6, 0, CrosswordDirection.Down),
        new CrosswordWordEntry(9, "INSPIRAR", "This means to fill someone with the urge or ability to do something creative.", 7, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(10, "DOBLE", "This means twice as much, or two of something.", 9, 3, CrosswordDirection.Across),
    });
}
