using System.Collections.Generic;

/// <summary>
/// Cebuano_Level9
/// Hardcoded crossword puzzle data for Cebuano level 9 (7 across,
/// 6 down; 5-8 letter words). Grid position is 0-indexed (row 0 = top,
/// col 0 = left); any cell not covered by a word below is a black/blocked cell, derived
/// at runtime rather than stored here. See CrosswordPuzzleData.cs for the shared types.
/// </summary>
public static class Cebuano_Level9
{
    public static readonly CrosswordPuzzle Data = new CrosswordPuzzle("Cebuano", 9, new List<CrosswordWordEntry>
    {
        new CrosswordWordEntry(1, "PULTA", "This is the opening used to enter or leave a room or building.", 0, 1, CrosswordDirection.Down),
        new CrosswordWordEntry(2, "BUSYU", "This is a swelling in the neck caused by an enlarged thyroid gland.", 0, 3, CrosswordDirection.Down),
        new CrosswordWordEntry(3, "HUKUT", "This means to bind something securely with rope or cord.", 1, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(4, "SAYUD", "This means to have information or understanding about something.", 4, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(5, "DAKUN", "This pronoun is used by a speaker to refer to themselves.", 4, 4, CrosswordDirection.Down),
        new CrosswordWordEntry(6, "BINUG", "This describes soft, waterlogged soil that squishes underfoot.", 6, 1, CrosswordDirection.Down),
        new CrosswordWordEntry(7, "PITSU", "This is the front chest portion of a bird's body.", 7, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(8, "AGILA", "This is a large bird of prey known for its sharp eyesight and powerful flight.", 10, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(9, "LINTI", "This is a curved piece of glass used to make objects look bigger.", 10, 3, CrosswordDirection.Down),
        new CrosswordWordEntry(10, "PYANU", "This is a large musical instrument played by pressing black and white keys.", 12, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(11, "PUKIR", "This is a card game where players bet based on the strength of their hand.", 14, 0, CrosswordDirection.Across),
        new CrosswordWordEntry(12, "KALAG", "This is the spiritual part of a person believed to live on after death.", 14, 2, CrosswordDirection.Down),
        new CrosswordWordEntry(13, "PUGON", "This is the appliance used for cooking or heating food.", 18, 0, CrosswordDirection.Across),
    });
}
