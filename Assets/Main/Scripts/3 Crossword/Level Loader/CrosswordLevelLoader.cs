using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

// ============================================================
// CrosswordLevelLoader.cs
//
// Loads the 10 pre-made, pre-validated puzzles per language from
// puzzles_tagalog.json / puzzles_cebuano.json / puzzles_hiligaynon.json.
// Levels run 1 to 10: level 1 has 5 words, level 10 has 11 words, with
// the mix of Across/Down decided by however each word best crosses the
// others (not a fixed pattern).
//
// These were generated offline with CrosswordBuilder (the same algorithm
// in CrosswordPuzzleGenerator.cs) and checked for correctness — every
// word's start cell is properly numbered, no duplicate clues, no
// cross-language leaks. Use this loader for a fixed 10-level campaign;
// use CrosswordPuzzleGenerator directly if you want fresh puzzles
// generated on the fly instead.
//
// Setup: drag the three puzzles_*.json files onto the matching fields
// below, add this script to a GameObject, then call LoadLevel(...).
// Depends on the WordEntry / PlacedWord / GeneratedPuzzle / CrosswordLanguage
// types declared in CrosswordPuzzleGenerator.cs — keep both scripts
// in the project together.
// ============================================================

[Serializable]
class LevelWordJson
{
    public int number;
    public int row;
    public int col;
    public string answer;
    public string clue;
    public string definition;
}

[Serializable]
class LevelPuzzleJson
{
    public int level;
    public string language;
    public int size;
    public int targetWords;
    public int wordCount;
    public string[] rows;
    public List<LevelWordJson> across;
    public List<LevelWordJson> down;
}

public class CrosswordLevelLoader : MonoBehaviour
{
    [Header("Pre-made puzzle files (10 levels each)")]
    [SerializeField] TextAsset tagalogLevels;
    [SerializeField] TextAsset cebuanoLevels;
    [SerializeField] TextAsset hiligaynonLevels;

    readonly Dictionary<string, List<LevelPuzzleJson>> cache = new Dictionary<string, List<LevelPuzzleJson>>();

    // level: 1 to 10 (5 words up to 11 words; difficulty rises with level).
    // language: Random picks one of the three languages for this call.
    public GeneratedPuzzle LoadLevel(CrosswordLanguage language, int level)
    {
        if (language == CrosswordLanguage.Random)
        {
            var options = new[] { CrosswordLanguage.Tagalog, CrosswordLanguage.Cebuano, CrosswordLanguage.Hiligaynon };
            language = options[UnityEngine.Random.Range(0, options.Length)];
        }

        var levels = LoadLevels(language.ToString());
        var found = levels.FirstOrDefault(l => l.level == level);
        if (found == null)
        {
            Debug.LogError(language + " has no level " + level + " (levels run 1-10).");
            return null;
        }
        return ToPuzzle(found);
    }

    List<LevelPuzzleJson> LoadLevels(string lang)
    {
        List<LevelPuzzleJson> cached;
        if (cache.TryGetValue(lang, out cached)) return cached;

        TextAsset asset = null;
        if (lang == "Tagalog") asset = tagalogLevels;
        else if (lang == "Cebuano") asset = cebuanoLevels;
        else if (lang == "Hiligaynon") asset = hiligaynonLevels;

        if (asset == null)
        {
            Debug.LogError("No TextAsset assigned for " + lang + ". Drag puzzles_" + lang.ToLower() + ".json onto the matching field.");
            return new List<LevelPuzzleJson>();
        }

        var list = JsonConvert.DeserializeObject<List<LevelPuzzleJson>>(asset.text);
        cache[lang] = list;
        return list;
    }

    static GeneratedPuzzle ToPuzzle(LevelPuzzleJson src)
    {
        var puzzle = new GeneratedPuzzle
        {
            Language = src.language,
            Size = src.size,
            Rows = src.rows
        };

        foreach (var w in src.across) puzzle.Words.Add(ToWord(w, src.language, true));
        foreach (var w in src.down) puzzle.Words.Add(ToWord(w, src.language, false));

        return puzzle;
    }

    static PlacedWord ToWord(LevelWordJson w, string language, bool across)
    {
        return new PlacedWord
        {
            Entry = new WordEntry { word = w.answer, translation = w.clue, definition = w.definition, letters = w.answer.Length, language = language },
            Row = w.row,
            Col = w.col,
            Across = across,
            Number = w.number
        };
    }
}
