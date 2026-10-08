/// <summary>
/// CrosswordPuzzleLibrary
/// Looks up the hardcoded CrosswordPuzzle data (Assets/Main/Data/Crossword/Legacy levels/&lt;Language&gt;/
/// "&lt;language&gt; - level N.cs") for a given PuzzleLanguage + level (1-10). PuzzleLanguage.Ilonggo
/// maps to the Hiligaynon data files/classes - "Ilonggo" is just the player-facing name for
/// that language used elsewhere in the project (see LanguageSelector.cs); the data itself
/// lives under Assets/Main/Data/Crossword/Legacy levels/Hiligaynon and the classes are named
/// Hiligaynon_LevelN, not Ilonggo_LevelN.
/// </summary>
public static class CrosswordPuzzleLibrary
{
    public static CrosswordPuzzle Get(PuzzleLanguage language, int level)
    {
        switch (language)
        {
            case PuzzleLanguage.Cebuano:
                return GetCebuano(level);
            case PuzzleLanguage.Ilonggo:
                return GetHiligaynon(level);
            default:
                return GetTagalog(level);
        }
    }

    /// <summary>The static class name backing this (language, level) pair, e.g. "Hiligaynon_Level3" -
    /// for display purposes (CrosswordAnswerKey shows this so a tester can find the source file).</summary>
    public static string GetSourceName(PuzzleLanguage language, int level)
    {
        string prefix = language == PuzzleLanguage.Ilonggo ? "Hiligaynon" : language.ToString();
        return prefix + "_Level" + level;
    }

    private static CrosswordPuzzle GetCebuano(int level)
    {
        switch (level)
        {
            case 1: return Cebuano_Level1.Data;
            case 2: return Cebuano_Level2.Data;
            case 3: return Cebuano_Level3.Data;
            case 4: return Cebuano_Level4.Data;
            case 5: return Cebuano_Level5.Data;
            case 6: return Cebuano_Level6.Data;
            case 7: return Cebuano_Level7.Data;
            case 8: return Cebuano_Level8.Data;
            case 9: return Cebuano_Level9.Data;
            default: return Cebuano_Level10.Data;
        }
    }

    private static CrosswordPuzzle GetHiligaynon(int level)
    {
        switch (level)
        {
            case 1: return Hiligaynon_Level1.Data;
            case 2: return Hiligaynon_Level2.Data;
            case 3: return Hiligaynon_Level3.Data;
            case 4: return Hiligaynon_Level4.Data;
            case 5: return Hiligaynon_Level5.Data;
            case 6: return Hiligaynon_Level6.Data;
            case 7: return Hiligaynon_Level7.Data;
            case 8: return Hiligaynon_Level8.Data;
            case 9: return Hiligaynon_Level9.Data;
            default: return Hiligaynon_Level10.Data;
        }
    }

    private static CrosswordPuzzle GetTagalog(int level)
    {
        switch (level)
        {
            case 1: return Tagalog_Level1.Data;
            case 2: return Tagalog_Level2.Data;
            case 3: return Tagalog_Level3.Data;
            case 4: return Tagalog_Level4.Data;
            case 5: return Tagalog_Level5.Data;
            case 6: return Tagalog_Level6.Data;
            case 7: return Tagalog_Level7.Data;
            case 8: return Tagalog_Level8.Data;
            case 9: return Tagalog_Level9.Data;
            default: return Tagalog_Level10.Data;
        }
    }
}
