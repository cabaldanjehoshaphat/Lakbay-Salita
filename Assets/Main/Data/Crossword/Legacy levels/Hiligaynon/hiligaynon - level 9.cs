using System.Collections.Generic;

/// <summary>
/// Hiligaynon_Level9
/// Hardcoded crossword puzzle data for Hiligaynon level 9 (7 across,
/// 6 down; 5-8 letter words). Grid position is 0-indexed (row 0 = top,
/// col 0 = left); any cell not covered by a word below is a black/blocked cell, derived
/// at runtime rather than stored here. See CrosswordPuzzleData.cs for the shared types.
/// </summary>
public static class Hiligaynon_Level9
{
    public static readonly CrosswordPuzzle Data = new CrosswordPuzzle("Hiligaynon", 9, new List<CrosswordWordEntry>
    {
        new CrosswordWordEntry(1, "KOTSYAM", "This is a particular variety of rice grain.", 0, 4, CrosswordDirection.Across),
        new CrosswordWordEntry(1, "KAROSA", "This is a wheeled vehicle, often pulled by an animal, used to carry loads.", 0, 4, CrosswordDirection.Down),
        new CrosswordWordEntry(2, "SISTA", "This is an ancient rattle-like musical instrument shaken by hand.", 0, 7, CrosswordDirection.Down),
        new CrosswordWordEntry(3, "MARPIL", "This is the hard white material from an elephant's tusks.", 0, 10, CrosswordDirection.Down),
        new CrosswordWordEntry(4, "GAROK", "This describes something that has spoiled and broken down over time.", 0, 13, CrosswordDirection.Down),
        new CrosswordWordEntry(5, "RIBIT", "This means to accidentally let something fall.", 0, 15, CrosswordDirection.Down),
        new CrosswordWordEntry(6, "KARBAW", "This is a large, horned work animal common on farms.", 1, 2, CrosswordDirection.Down),
        new CrosswordWordEntry(7, "DALIA", "This is a brightly colored, many-petaled garden flower.", 1, 12, CrosswordDirection.Across),
        new CrosswordWordEntry(8, "MADRE", "This is the female parent of a child.", 2, 1, CrosswordDirection.Across),
        new CrosswordWordEntry(9, "SOBRA", "This describes an amount that goes beyond what is needed or expected.", 2, 7, CrosswordDirection.Across),
        new CrosswordWordEntry(10, "OBIHA", "This is a woolly farm animal, specifically the female of its kind.", 3, 13, CrosswordDirection.Across),
        new CrosswordWordEntry(11, "KABESERA", "This is the main city of a country or region, often its seat of government.", 4, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(12, "WISIK", "This is the act of scattering small drops of liquid over something.", 4, 9, CrosswordDirection.Across),
    });
}
