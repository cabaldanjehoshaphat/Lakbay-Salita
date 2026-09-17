using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// WordleWordVerifier
/// Plain data shape for a puzzle's target-word JSON file (e.g.
/// Assets/Main/Scripts/2 Wordle/1 - Word - Script - Cebuano/Cebuano_AKO.json).
/// </summary>
[System.Serializable]
public class WordleWordData
{
    public string word;
    public string translation;
    public string definition;
    public int letters;
    public string language;
}

/// <summary>
/// WordleWordVerifier
/// Verifies one completed row (called by WordleKeyboardTyper when Enter is pressed on a
/// full row) against a target word loaded from a JSON file, using the same duplicate-letter
/// -safe two-pass scoring Assets/Sub/Wordle_Imported/Scripts/Board.cs already uses:
/// exact-position matches first (green), then letters present elsewhere in the word
/// (yellow), then everything left over — letters not in the word at all (red). This
/// version uses plain background-color changes and the cell's normal TMP font for the
/// letter — no ImageContainer/sprite dependency. Verification runs row by row from the
/// first row up to the last, and returns true (a win) once a row's guess exactly matches
/// the target word, which WordleKeyboardTyper uses to stop the game.
///
/// The loaded word and its definition are shown as read-only fields in this component's
/// Inspector (on the same "WordleGenerator" GameObject as WordleRowsColumnGenerator),
/// refreshed automatically via OnValidate whenever verificationWordJson is assigned or
/// changed — no need to enter Play mode to see them.
/// </summary>
public class WordleWordVerifier : MonoBehaviour
{
    [Tooltip("JSON file (e.g. Cebuano_AKO.json) containing the target word for this puzzle.")]
    public TextAsset verificationWordJson;

    [Tooltip("The generator whose Cells this verifies against.")]
    public WordleRowsColumnGenerator generator;

    [Header("Loaded Word Info (read-only, from verificationWordJson)")]
    [Tooltip("The target word, loaded from verificationWordJson.")]
    [SerializeField] private string word;

    [Tooltip("The target word's definition, loaded from verificationWordJson.")]
    [SerializeField] [TextArea(2, 4)] private string definition;

    /// <summary>The target word's definition (read-only), for UI display (e.g. Dialog Panel).</summary>
    public string Definition => definition;

    [Header("Colors")]
    [Tooltip("Background color for a letter that's correct and in the right position.")]
    public Color correctColor = new Color(0.42f, 0.73f, 0.42f); // green

    [Tooltip("Background color for a letter that's in the word but in the wrong position.")]
    public Color wrongPositionColor = new Color(0.80f, 0.73f, 0.30f); // yellow

    [Tooltip("Background color for a letter that isn't in the word at all.")]
    public Color notInWordColor = new Color(0.80f, 0.30f, 0.30f); // red

    private enum LetterState
    {
        Correct,
        WrongPosition,
        NotInWord
    }

    private string targetWord;

    private void Awake()
    {
        LoadTargetWord();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        LoadTargetWord();
    }
#endif

    private void LoadTargetWord()
    {
        if (verificationWordJson == null)
        {
            Debug.LogWarning("WordleWordVerifier: no verificationWordJson assigned.");
            return;
        }

        WordleWordData data = JsonUtility.FromJson<WordleWordData>(verificationWordJson.text);
        targetWord = data.word.ToUpperInvariant();
        word = data.word;
        definition = data.definition;
    }

    /// <summary>
    /// Verifies one completed row (0-based rowIndex) against the target word and colors its
    /// cells accordingly. Returns true if every letter in the row is an exact-position match
    /// (i.e. the word was correctly guessed).
    /// </summary>
    public bool VerifyRow(int rowIndex)
    {
        if (string.IsNullOrEmpty(targetWord) || generator == null)
        {
            return false;
        }

        int columns = generator.columns;
        int startIndex = rowIndex * columns;

        if (startIndex + columns > generator.Cells.Count)
        {
            return false;
        }

        char[] guess = new char[columns];
        for (int i = 0; i < columns; i++)
        {
            string cellText = generator.Cells[startIndex + i].text;
            guess[i] = string.IsNullOrEmpty(cellText) ? '\0' : cellText[0];
        }

        var state = new LetterState[columns];
        bool[] targetLetterUsed = new bool[targetWord.Length];
        bool[] guessLetterMatched = new bool[columns];

        // Pass 1: exact position matches (green).
        for (int i = 0; i < columns; i++)
        {
            if (i < targetWord.Length && guess[i] == targetWord[i])
            {
                state[i] = LetterState.Correct;
                guessLetterMatched[i] = true;
                targetLetterUsed[i] = true;
            }
        }

        // Pass 2: letter exists elsewhere in the word (yellow), otherwise not in the word at
        // all (red). Each target letter can only satisfy one guess letter, so duplicate
        // letters are handled the same way Board.cs already does.
        for (int i = 0; i < columns; i++)
        {
            if (guessLetterMatched[i])
            {
                continue;
            }

            int foundAt = -1;
            for (int j = 0; j < targetWord.Length; j++)
            {
                if (!targetLetterUsed[j] && targetWord[j] == guess[i])
                {
                    foundAt = j;
                    break;
                }
            }

            if (foundAt >= 0)
            {
                state[i] = LetterState.WrongPosition;
                targetLetterUsed[foundAt] = true;
            }
            else
            {
                state[i] = LetterState.NotInWord;
            }
        }

        bool allCorrect = columns == targetWord.Length;
        for (int i = 0; i < columns; i++)
        {
            ApplyColor(generator.Cells[startIndex + i], state[i]);
            if (state[i] != LetterState.Correct)
            {
                allCorrect = false;
            }
        }

        return allCorrect;
    }

    /// <summary>Colors the cell's background Image according to its verification state
    /// (green/yellow/red). The Image is the cell's parent, since the TMP_Text lives inside
    /// the cell as a child.</summary>
    private void ApplyColor(TMP_Text cellText, LetterState state)
    {
        if (cellText == null || cellText.transform.parent == null)
        {
            return;
        }

        Image cellImage = cellText.transform.parent.GetComponent<Image>();
        if (cellImage == null)
        {
            return;
        }

        switch (state)
        {
            case LetterState.Correct:
                cellImage.color = correctColor;
                break;
            case LetterState.WrongPosition:
                cellImage.color = wrongPositionColor;
                break;
            default:
                cellImage.color = notInWordColor;
                break;
        }
    }
}
