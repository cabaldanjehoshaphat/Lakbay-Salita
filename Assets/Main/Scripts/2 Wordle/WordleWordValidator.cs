using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// WordleWordValidator
/// Answers "is this a real word?" for the Wordle screen, using the same dictionary JSON files as the Library
/// (Assets/Main/Data/Language Dictionary/cebuano.json, hiligaynon.json, tagalog.json: a JSON array of entries with a "word" field).
/// Only words with exactly the grid's number of letters are kept, as upper-case letters without spaces, hyphens or accents
/// marks beyond the letters themselves. Parsing the file takes a moment, so WordleGameController calls Load() from a
/// coroutine after the screen is shown; until it is ready, Contains() accepts every word so nobody is blocked.
/// The puzzle's own target word is always accepted, even if the dictionary does not list it.
/// </summary>
public class WordleWordValidator
{
    private readonly HashSet<string> _words = new HashSet<string>();
    private readonly string _extra;

    /// <summary>True once the dictionary was parsed.</summary>
    public bool Ready { get; private set; }

    /// <summary>Number of words kept for this grid size.</summary>
    public int Count
    {
        get { return _words.Count; }
    }

    /// <param name="extraAccepted">A word that is always accepted (the puzzle's target word).</param>
    public WordleWordValidator(string extraAccepted)
    {
        _extra = Normalize(extraAccepted);
    }

    /// <summary>Parses the dictionary JSON and keeps the words that are exactly "length" letters long.</summary>
    public void Load(string json, int length)
    {
        _words.Clear();
        if (!string.IsNullOrEmpty(json))
        {
            string text = json.TrimStart('﻿', ' ', '\r', '\n', '\t');
            RawDictFile file = JsonUtility.FromJson<RawDictFile>("{\"items\":" + text + "}");
            if (file != null && file.items != null)
            {
                foreach (RawDictEntry entry in file.items)
                {
                    if (entry == null || string.IsNullOrEmpty(entry.word))
                    {
                        continue;
                    }
                    string w = Normalize(entry.word);
                    if (w.Length == length)
                    {
                        _words.Add(w);
                    }
                }
            }
        }
        Ready = _words.Count > 0;
    }

    /// <summary>True if the guess is a known word, is the target word, or the dictionary is not ready yet.</summary>
    public bool Contains(string guess)
    {
        string g = Normalize(guess);
        if (g.Length == 0)
        {
            return false;
        }
        if (!Ready || g == _extra)
        {
            return true;
        }
        return _words.Contains(g);
    }

    private static string Normalize(string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return string.Empty;
        }
        var sb = new System.Text.StringBuilder(s.Length);
        foreach (char c in s)
        {
            if (char.IsLetter(c))
            {
                sb.Append(char.ToUpperInvariant(c));
            }
        }
        return sb.ToString();
    }
}
