using System.Collections.Generic;

/// <summary>
/// Tagalog_Level8
/// Hardcoded crossword puzzle data for Tagalog level 8 (6 across,
/// 6 down; 4-8 letter words). Grid position is 0-indexed (row 0 = top,
/// col 0 = left); any cell not covered by a word below is a black/blocked cell, derived
/// at runtime rather than stored here. See CrosswordPuzzleData.cs for the shared types.
/// </summary>
public static class Tagalog_Level8
{
    public static readonly CrosswordPuzzle Data = new CrosswordPuzzle("Tagalog", 8, new List<CrosswordWordEntry>
    {
        new CrosswordWordEntry(1, "PAGANYAN", "This word is used to describe something happening in that particular way.", 0, 1, CrosswordDirection.Across),
        new CrosswordWordEntry(2, "YODO", "This is a chemical element used as an antiseptic for wounds.", 0, 6, CrosswordDirection.Down),
        new CrosswordWordEntry(3, "SUBIDO", "This describes light or color that is extremely vivid and intense.", 2, 2, CrosswordDirection.Across),
        new CrosswordWordEntry(4, "UMISTIMA", "This means to provide enjoyment or amusement for someone.", 2, 3, CrosswordDirection.Down),
        new CrosswordWordEntry(5, "PAMBAYOK", "This is a device or tool that produces a rapid shaking motion.", 3, 9, CrosswordDirection.Down),
        new CrosswordWordEntry(6, "PANSIPIT", "This is a device set to catch and kill rats.", 5, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(6, "PERAS", "This is a sweet, bell-shaped fruit that grows on trees.", 5, 0, CrosswordDirection.Down),
        new CrosswordWordEntry(7, "PAGLINGO", "This is the deliberate killing of a prominent or important person.", 5, 5, CrosswordDirection.Down),
        new CrosswordWordEntry(8, "TRAPO", "This is a piece of old cloth used for cleaning.", 5, 7, CrosswordDirection.Down),
        new CrosswordWordEntry(9, "TINGKALA", "This is the ability to understand something fully.", 7, 2, CrosswordDirection.Across),
        new CrosswordWordEntry(10, "SEDATIBO", "This is a medicine used to calm a person or help them sleep.", 9, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(11, "MAHIGOP", "This describes having the ability to drink a liquid slowly in small amounts.", 11, 1, CrosswordDirection.Across),
    });
}
