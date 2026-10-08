using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CrosswordCategoryData
/// The data model and loader for the category crosswords. The puzzles live in one JSON file per language,
/// Assets/Main/Data/Crossword/Puzzles/crossword_puzzles_<language>.json, found through LevelDataRegistry: 3 languages (0 Cebuano, 1 Ilonggo, 2 Tagalog) x 7 categories x 3 sizes (Small 5 words, Medium 8, Large 11). Every
/// answer of a puzzle belongs to its category. A word has its grid position (row and col counted from the top-left, across = true for an
/// across word and false for a down word), its clue number and its clue. CrosswordCategoryData.Puzzle(...) returns a puzzle definition and
/// ToPuzzle turns it into the CrosswordPuzzle the game already plays (CrosswordInputController.Initialize). The JSON is read once and
/// cached. The grids were generated, so to change a word ask for the puzzles to be rebuilt instead of moving letters by hand.
/// </summary>
[Serializable]
public class CrosswordWordDef
{
    public string answer;
    public string clue;
    public int row;
    public int col;
    public bool across;
    public int number;
}

[Serializable]
public class CrosswordPuzzleDef
{
    public string size;
    public List<CrosswordWordDef> words = new List<CrosswordWordDef>();
}

[Serializable]
public class CrosswordCategoryDef
{
    public string name;
    public List<CrosswordPuzzleDef> puzzles = new List<CrosswordPuzzleDef>();
}

[Serializable]
public class CrosswordLanguageDef
{
    public string name;
    public List<CrosswordCategoryDef> categories = new List<CrosswordCategoryDef>();
}

[Serializable]
public class CrosswordCategoriesFile
{
    public string purpose;
    public string status;
    public List<CrosswordLanguageDef> languages = new List<CrosswordLanguageDef>();
}

public static class CrosswordCategoryData
{
    private static CrosswordCategoriesFile _file;

    /// <summary>Language names in the order of the language index (0 Cebuano, 1 Ilonggo, 2 Tagalog).</summary>
    public static readonly string[] LanguageNames = { "Cebuano", "Ilonggo", "Tagalog" };

    /// <summary>The whole puzzle file (null if it is missing).</summary>
    public static CrosswordCategoriesFile Data
    {
        get
        {
            if (_file == null)
            {
                LevelDataRegistry registry = LevelDataRegistry.Instance;
                if (registry == null)
                {
                    return null;
                }
                // one JSON per language, in language order (see LevelDataRegistry)
                _file = new CrosswordCategoriesFile();
                _file.languages.AddRange(LevelDataRegistry.Parse<CrosswordLanguageDef>(registry.crosswordFiles, "Crossword"));
            }
            return _file;
        }
    }

    /// <summary>Forgets the cached file so the next call reads the JSON again.</summary>
    public static void Reload()
    {
        _file = null;
    }

    public static CrosswordLanguageDef Language(int language)
    {
        CrosswordCategoriesFile file = Data;
        if (file == null || language < 0 || language >= file.languages.Count)
        {
            return null;
        }
        return file.languages[language];
    }

    public static int CategoryCount(int language)
    {
        CrosswordLanguageDef l = Language(language);
        return l != null ? l.categories.Count : 0;
    }

    public static CrosswordCategoryDef Category(int language, int category)
    {
        CrosswordLanguageDef l = Language(language);
        if (l == null || category < 0 || category >= l.categories.Count)
        {
            return null;
        }
        return l.categories[category];
    }

    public static int SizeCount(int language, int category)
    {
        CrosswordCategoryDef c = Category(language, category);
        return c != null ? c.puzzles.Count : 0;
    }

    public static CrosswordPuzzleDef Puzzle(int language, int category, int size)
    {
        CrosswordCategoryDef c = Category(language, category);
        if (c == null || size < 0 || size >= c.puzzles.Count)
        {
            return null;
        }
        return c.puzzles[size];
    }

    /// <summary>How many puzzles a language has in total (all categories and sizes).</summary>
    public static int TotalPuzzles(int language)
    {
        int total = 0;
        for (int c = 0; c < CategoryCount(language); c++)
        {
            total += SizeCount(language, c);
        }
        return total;
    }

    /// <summary>Turns a puzzle definition into the CrosswordPuzzle the crossword game plays. The "level" is the size number (1 Small, 2 Medium, 3 Large).</summary>
    public static CrosswordPuzzle ToPuzzle(int language, int size, CrosswordPuzzleDef def)
    {
        var words = new List<CrosswordWordEntry>();
        foreach (CrosswordWordDef w in def.words)
        {
            words.Add(new CrosswordWordEntry(w.number, w.answer, w.clue, w.row, w.col,
                w.across ? CrosswordDirection.Across : CrosswordDirection.Down));
        }
        return new CrosswordPuzzle(LanguageNames[Mathf.Clamp(language, 0, 2)], size + 1, words);
    }
}
