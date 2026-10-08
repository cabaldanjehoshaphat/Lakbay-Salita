using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// WordleData
/// The data model and loader for the category levels of Wordle. The levels live in one JSON file per language,
/// Assets/Main/Data/Wordle/Levels/wordle_levels_<language>.json, found through LevelDataRegistry: 3 languages (0 Cebuano, 1 Ilonggo, 2 Tagalog) x 7 categories x 6 levels. A level is one
/// word (4-7 letters, longer with every level) with its English meaning and a dictionary clue. WordleData.Level(language, category, level)
/// returns one level; the file is read once and cached. To change a word, edit the JSON (letters A-Z only, categories in the same order
/// as the Word Search categories). CategoryColors holds the colour dot of each category, shared by the start screen and the play screen.
/// </summary>
[Serializable]
public class WordleLevelDef
{
    public string word;
    public string meaning;
    public string clue;
}

[Serializable]
public class WordleCategoryDef
{
    public string name;
    public List<WordleLevelDef> levels = new List<WordleLevelDef>();
}

[Serializable]
public class WordleLanguageDef
{
    public string name;
    public List<WordleCategoryDef> categories = new List<WordleCategoryDef>();
}

[Serializable]
public class WordleLevelsFile
{
    public string purpose;
    public string status;
    public List<WordleLanguageDef> languages = new List<WordleLanguageDef>();
}

public static class WordleData
{
    private static WordleLevelsFile _file;

    /// <summary>Language names in the order of the language index (0 Cebuano, 1 Ilonggo, 2 Tagalog).</summary>
    public static readonly string[] LanguageNames = { "Cebuano", "Ilonggo", "Tagalog" };

    /// <summary>The colour of each category's dot (Animals, Food and drink, Body, Family and people, Home and things, Nature and weather, Numbers).</summary>
    public static readonly Color[] CategoryColors =
    {
        new Color32(217, 96, 59, 255), new Color32(232, 185, 35, 255), new Color32(93, 160, 42, 255), new Color32(138, 63, 199, 255),
        new Color32(55, 138, 221, 255), new Color32(29, 158, 117, 255), new Color32(232, 117, 26, 255)
    };

    /// <summary>The whole level file (null if it is missing).</summary>
    public static WordleLevelsFile Data
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
                _file = new WordleLevelsFile();
                _file.languages.AddRange(LevelDataRegistry.Parse<WordleLanguageDef>(registry.wordleFiles, "Wordle"));
            }
            return _file;
        }
    }

    /// <summary>Forgets the cached file so the next call reads the JSON again.</summary>
    public static void Reload()
    {
        _file = null;
    }

    public static WordleLanguageDef Language(int language)
    {
        WordleLevelsFile file = Data;
        if (file == null || language < 0 || language >= file.languages.Count)
        {
            return null;
        }
        return file.languages[language];
    }

    public static int CategoryCount(int language)
    {
        WordleLanguageDef l = Language(language);
        return l != null ? l.categories.Count : 0;
    }

    public static WordleCategoryDef Category(int language, int category)
    {
        WordleLanguageDef l = Language(language);
        if (l == null || category < 0 || category >= l.categories.Count)
        {
            return null;
        }
        return l.categories[category];
    }

    public static int LevelCount(int language, int category)
    {
        WordleCategoryDef c = Category(language, category);
        return c != null ? c.levels.Count : 0;
    }

    public static WordleLevelDef Level(int language, int category, int level)
    {
        WordleCategoryDef c = Category(language, category);
        if (c == null || level < 0 || level >= c.levels.Count)
        {
            return null;
        }
        return c.levels[level];
    }

    /// <summary>The colour dot of a category (wraps around if there are more categories than colours).</summary>
    public static Color CategoryColor(int category)
    {
        return CategoryColors[Mathf.Abs(category) % CategoryColors.Length];
    }

    /// <summary>How many levels a language has in total (all categories).</summary>
    public static int TotalLevels(int language)
    {
        int total = 0;
        for (int c = 0; c < CategoryCount(language); c++)
        {
            total += LevelCount(language, c);
        }
        return total;
    }
}
