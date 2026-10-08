using System.Collections.Generic;

/// <summary>
/// CrosswordPuzzleData
/// Shared data types for the hardcoded per-level crossword puzzles in Assets/Main/Data
/// (e.g. "cebuano - level 1.cs" through "tagalog - level 10.cs"). Each puzzle file defines
/// one static CrosswordPuzzle listing its Across/Down words with a grid position (0-indexed,
/// row 0 = top, col 0 = left) - any cell not covered by a word's letters is a black/blocked
/// cell, meant to be derived at runtime by whatever generator consumes this data rather than
/// being stored explicitly here. "number" follows standard crossword numbering (top-left to
/// bottom-right scan; a cell is numbered once if it starts an Across and/or Down entry).
/// </summary>
public enum CrosswordDirection
{
    Across,
    Down
}

[System.Serializable]
public class CrosswordWordEntry
{
    public int number;
    public string answer;
    public string clue;
    public int row;
    public int col;
    public CrosswordDirection direction;

    public CrosswordWordEntry(int number, string answer, string clue, int row, int col, CrosswordDirection direction)
    {
        this.number = number;
        this.answer = answer;
        this.clue = clue;
        this.row = row;
        this.col = col;
        this.direction = direction;
    }
}

public class CrosswordPuzzle
{
    public string language;
    public int level;
    public List<CrosswordWordEntry> words;

    public CrosswordPuzzle(string language, int level, List<CrosswordWordEntry> words)
    {
        this.language = language;
        this.level = level;
        this.words = words;
    }
}
