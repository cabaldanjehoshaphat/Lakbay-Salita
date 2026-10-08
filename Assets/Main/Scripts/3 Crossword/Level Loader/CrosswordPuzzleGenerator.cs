using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

// ============================================================
// CrosswordPuzzleGenerator.cs
//
// Builds crossword puzzles from the Tagalog / Cebuano / Hiligaynon
// word banks (fields: word, translation, definition, letters, language).
//
// IMPORTANT FIX vs. the previous version of this script:
// the old placement check only compared letters, so it could stack two
// words running the SAME direction on top of each other in one column/row
// (e.g. "IYARI" and "YARI" both Down, overlapping instead of crossing) —
// that produced a broken, unnumberable puzzle. This version tracks which
// direction (Across/Down) owns each cell and only allows a new word to
// reuse a cell that belongs to the OPPOSITE direction, i.e. a real crossing.
//
// Setup:
//  1. Package Manager > Add package by name > com.unity.nuget.newtonsoft-json
//  2. Drag tagalog.json, cebuano.json and hiligaynon.json onto the three
//     TextAsset fields below.
//  3. Add this script to an empty GameObject and press Play.
//     Puzzle.Rows / Puzzle.AcrossWords / Puzzle.DownWords are ready
//     to feed into a grid-drawing script.
//
// For 10 ready-made, pre-validated puzzles per language (levels 1-10,
// 5 up to 11 words) see CrosswordLevelLoader.cs + puzzles_<language>.json
// instead — this script is for generating NEW puzzles on the fly.
// ============================================================

[Serializable]
public class WordEntry
{
    public string word;         // "BAHAY"  (A-Z only, uppercase)
    public string translation;  // short clue text
    public string definition;   // longer clue text
    public int letters;
    public string language;     // "Tagalog" | "Cebuano" | "Hiligaynon"

    public bool HasClue()
    {
        return !string.IsNullOrWhiteSpace(translation);
    }
}

public class PlacedWord
{
    public WordEntry Entry;
    public int Row;
    public int Col;
    public bool Across;
    public int Number;

    public string Text { get { return Entry.word; } }
}

public class GeneratedPuzzle
{
    public string Language;
    public int Size;
    public string[] Rows;                              // '#' = block, letter = solution
    public List<PlacedWord> Words = new List<PlacedWord>();
    public int Crossings;

    public List<PlacedWord> AcrossWords
    {
        get { return Words.Where(w => w.Across).OrderBy(w => w.Number).ToList(); }
    }

    public List<PlacedWord> DownWords
    {
        get { return Words.Where(w => !w.Across).OrderBy(w => w.Number).ToList(); }
    }
}

public enum CrosswordLanguage { Random, Tagalog, Cebuano, Hiligaynon }

public class CrosswordPuzzleGenerator : MonoBehaviour
{
    [Header("Word banks (drag the matching .json here)")]
    [SerializeField] TextAsset tagalogBank;
    [SerializeField] TextAsset cebuanoBank;
    [SerializeField] TextAsset hiligaynonBank;

    [Header("Puzzle settings")]
    [SerializeField] CrosswordLanguage language = CrosswordLanguage.Random;
    [SerializeField] int gridSize = 7;                   // 5 to 11 all work
    [SerializeField] int targetWordCount = 7;             // how many words to try to fit
    [SerializeField] int seed = 0;                        // 0 = a new random puzzle every run
    [SerializeField] int attemptsPerTry = 6;               // internal restarts per generation try
    [SerializeField] int candidatesPerStep = 500;          // words considered at each placement step
    [SerializeField] int maxSeedTries = 40;                // extra full retries if target isn't reached

    public GeneratedPuzzle Puzzle { get; private set; }

    readonly Dictionary<string, List<WordEntry>> cache = new Dictionary<string, List<WordEntry>>();

    void Start()
    {
        Puzzle = GenerateOne();
        LogPuzzle(Puzzle);
    }

    // Generates exactly one puzzle using the Inspector settings above.
    public GeneratedPuzzle GenerateOne()
    {
        int baseSeed = seed != 0 ? seed : Environment.TickCount;
        string lang = ResolveLanguage(baseSeed);
        var bank = LoadBank(lang).Where(e => e.HasClue()).ToList();

        // Try a handful of independent seeds and keep whichever gets
        // closest to (or reaches) the target word count.
        GeneratedPuzzle best = null;
        for (int i = 0; i < maxSeedTries; i++)
        {
            var candidate = CrosswordBuilder.Generate(bank, gridSize, targetWordCount, baseSeed + i, attemptsPerTry, candidatesPerStep);
            if (best == null || candidate.Words.Count > best.Words.Count ||
                (candidate.Words.Count == best.Words.Count && candidate.Crossings > best.Crossings))
                best = candidate;
            if (best.Words.Count >= targetWordCount) break;
        }

        best.Language = lang;
        return best;
    }

    // Random picks one of the three languages, keyed off the same seed
    // (so a given seed always reproduces the same language + the same puzzle).
    string ResolveLanguage(int useSeed)
    {
        if (language != CrosswordLanguage.Random) return language.ToString();

        var options = new[] { "Tagalog", "Cebuano", "Hiligaynon" };
        return options[new System.Random(useSeed).Next(options.Length)];
    }

    List<WordEntry> LoadBank(string lang)
    {
        List<WordEntry> cached;
        if (cache.TryGetValue(lang, out cached)) return cached;

        TextAsset asset = null;
        if (lang == "Tagalog") asset = tagalogBank;
        else if (lang == "Cebuano") asset = cebuanoBank;
        else if (lang == "Hiligaynon") asset = hiligaynonBank;

        if (asset == null)
        {
            Debug.LogError("No TextAsset assigned for " + lang + ". Drag " + lang.ToLower() + ".json onto the matching field.");
            return new List<WordEntry>();
        }

        var list = JsonConvert.DeserializeObject<List<WordEntry>>(asset.text);
        cache[lang] = list;
        return list;
    }

    void LogPuzzle(GeneratedPuzzle p)
    {
        Debug.Log("--- " + p.Language + " crossword, " + p.Size + "x" + p.Size + ", " + p.Words.Count + " words ---");
        foreach (var row in p.Rows) Debug.Log(row);
        foreach (var w in p.AcrossWords) Debug.Log(w.Number + " Across: " + w.Entry.translation + "  (" + w.Text + ")");
        foreach (var w in p.DownWords) Debug.Log(w.Number + " Down: " + w.Entry.translation + "  (" + w.Text + ")");
    }
}

// ----------------------------------------------------------------
// Pure generation algorithm — no Unity types, so it can be unit
// tested or reused outside Unity too.
// ----------------------------------------------------------------
public static class CrosswordBuilder
{
    // size: 5-11 all work. targetWords: stop early once this many words fit.
    // Same seed = same puzzle (useful for a "daily puzzle" feature).
    public static GeneratedPuzzle Generate(List<WordEntry> bank, int size, int targetWords, int seed, int attempts = 6, int candidatesPerStep = 500)
    {
        var rng = new System.Random(seed);
        var pool = bank.Where(e => e.word.Length >= 3 && e.word.Length <= size).ToList();

        GeneratedPuzzle best = null;
        for (int i = 0; i < attempts; i++)
        {
            var candidate = BuildOnce(pool, size, rng, targetWords, candidatesPerStep);
            if (best == null || Score(candidate) > Score(best)) best = candidate;
            if (best.Words.Count >= targetWords) break;
        }

        Finish(best);
        return best;
    }

    static int Score(GeneratedPuzzle p)
    {
        return p.Words.Count * 100 + p.Crossings;
    }

    // Greedy fill: at every step, look at a fresh random sample of the whole
    // pool and place whichever word crosses the existing grid the most.
    // Re-sampling each step (rather than fixing one small subset up front)
    // is what lets 8-11 word puzzles fit into modest grids.
    static GeneratedPuzzle BuildOnce(List<WordEntry> pool, int size, System.Random rng, int targetWords, int candidatesPerStep)
    {
        var grid = new char[size, size];
        var dirGrid = new int[size, size];          // 1 = Across owns this cell, 2 = Down, 3 = both
        var puzzle = new GeneratedPuzzle { Size = size };
        var usedClues = new HashSet<string>();
        var usedWords = new HashSet<string>();

        var shuffled = pool.OrderBy(e => rng.Next()).ToList();
        var first = shuffled.FirstOrDefault(e => e.word.Length >= System.Math.Min(5, size)) ?? shuffled[0];

        int r0 = size / 2;
        int c0 = (size - first.word.Length) / 2;
        Put(grid, dirGrid, first.word, r0, c0, true);
        puzzle.Words.Add(new PlacedWord { Entry = first, Row = r0, Col = c0, Across = true });
        usedClues.Add(first.translation.ToLowerInvariant());
        usedWords.Add(first.word);

        while (puzzle.Words.Count < targetWords)
        {
            var sample = pool.OrderBy(e => rng.Next()).Take(candidatesPerStep);

            WordEntry bestWord = null;
            int bestR = 0, bestC = 0, bestCross = 0;
            bool bestAcross = true;

            foreach (var entry in sample)
            {
                if (usedWords.Contains(entry.word)) continue;
                string clueKey = entry.translation.ToLowerInvariant();
                if (usedClues.Contains(clueKey)) continue;

                for (int r = 0; r < size; r++)
                {
                    for (int c = 0; c < size; c++)
                    {
                        for (int d = 0; d < 2; d++)
                        {
                            bool across = d == 0;
                            int cross = TryPlace(grid, dirGrid, entry.word, r, c, across, size);
                            if (cross > bestCross)
                            {
                                bestCross = cross; bestWord = entry; bestR = r; bestC = c; bestAcross = across;
                            }
                        }
                    }
                }
            }

            if (bestWord == null) break;   // no candidate in this sample intersects — stop with what we have

            Put(grid, dirGrid, bestWord.word, bestR, bestC, bestAcross);
            puzzle.Words.Add(new PlacedWord { Entry = bestWord, Row = bestR, Col = bestC, Across = bestAcross });
            puzzle.Crossings += bestCross;
            usedClues.Add(bestWord.translation.ToLowerInvariant());
            usedWords.Add(bestWord.word);
        }

        return puzzle;
    }

    // Returns how many letters the word shares with existing words,
    // or -1 if it cannot be placed here. dirGrid marks, per cell, which
    // directions already occupy it (1=Across, 2=Down, 3=both) so a new
    // word can only reuse a cell owned by the OPPOSITE direction — two
    // words running the same direction are never allowed to overlap.
    static int TryPlace(char[,] g, int[,] dirGrid, string w, int r, int c, bool across, int size)
    {
        int dr = across ? 0 : 1;
        int dc = across ? 1 : 0;
        int myBit = across ? 1 : 2;
        int er = r + dr * (w.Length - 1);
        int ec = c + dc * (w.Length - 1);
        if (r < 0 || c < 0 || er >= size || ec >= size) return -1;

        if (Filled(g, r - dr, c - dc, size)) return -1;
        if (Filled(g, er + dr, ec + dc, size)) return -1;

        int crossings = 0;
        for (int i = 0; i < w.Length; i++)
        {
            int rr = r + dr * i;
            int cc = c + dc * i;
            char cur = g[rr, cc];

            if (cur == '\0')
            {
                int pr = dc;
                int pc = dr;
                if (Filled(g, rr + pr, cc + pc, size)) return -1;
                if (Filled(g, rr - pr, cc - pc, size)) return -1;
            }
            else if (cur == w[i])
            {
                if ((dirGrid[rr, cc] & myBit) != 0) return -1;   // same direction already runs through here
                crossings++;
            }
            else
            {
                return -1;
            }
        }
        return crossings;
    }

    static bool Filled(char[,] g, int r, int c, int size)
    {
        return r >= 0 && c >= 0 && r < size && c < size && g[r, c] != '\0';
    }

    static void Put(char[,] g, int[,] dirGrid, string w, int r, int c, bool across)
    {
        int dr = across ? 0 : 1;
        int dc = across ? 1 : 0;
        int myBit = across ? 1 : 2;
        for (int i = 0; i < w.Length; i++)
        {
            int rr = r + dr * i, cc = c + dc * i;
            g[rr, cc] = w[i];
            dirGrid[rr, cc] |= myBit;
        }
    }

    static void PutChars(char[,] g, string w, int r, int c, bool across)
    {
        for (int i = 0; i < w.Length; i++)
        {
            if (across) g[r, c + i] = w[i];
            else g[r + i, c] = w[i];
        }
    }

    // Builds the row strings and assigns clue numbers (reading order: left-to-right, top-to-bottom).
    static void Finish(GeneratedPuzzle p)
    {
        int n = p.Size;
        var g = new char[n, n];
        foreach (var word in p.Words) PutChars(g, word.Text, word.Row, word.Col, word.Across);

        p.Rows = new string[n];
        for (int r = 0; r < n; r++)
        {
            var chars = new char[n];
            for (int c = 0; c < n; c++) chars[c] = g[r, c] == '\0' ? '#' : g[r, c];
            p.Rows[r] = new string(chars);
        }

        var startNumber = new Dictionary<int, int>();
        int number = 0;
        for (int r = 0; r < n; r++)
        {
            for (int c = 0; c < n; c++)
            {
                bool has = g[r, c] != '\0';
                bool startsAcross = has && (c == 0 || g[r, c - 1] == '\0') && c + 1 < n && g[r, c + 1] != '\0';
                bool startsDown = has && (r == 0 || g[r - 1, c] == '\0') && r + 1 < n && g[r + 1, c] != '\0';
                if (startsAcross || startsDown) startNumber[r * n + c] = ++number;
            }
        }

        foreach (var word in p.Words)
        {
            int key = word.Row * n + word.Col;
            if (!startNumber.ContainsKey(key))
                throw new InvalidOperationException("Word " + word.Entry.word + " at r=" + word.Row + " c=" + word.Col + " has no numbered start cell — placement bug.");
        }
        foreach (var word in p.Words) word.Number = startNumber[word.Row * n + word.Col];
    }
}
