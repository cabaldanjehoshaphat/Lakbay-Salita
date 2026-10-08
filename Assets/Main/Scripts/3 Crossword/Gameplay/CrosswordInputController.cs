using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// CrosswordInputController
/// Orchestrates one crossword puzzle: tells CrosswordGridGenerator to build the grid, tells
/// CrosswordCluePanel to build the clue lists from the result, then owns the actual play
/// loop - which word/cell is active, keyboard typing and navigation, and syncing both the
/// grid's highlight and the clue panel to the current selection.
///
/// language picks which of Assets/Main/Data/Crossword/Legacy levels/&lt;Language&gt; this scene draws from
/// (PuzzleLanguage.Ilonggo maps to the Hiligaynon data - see CrosswordPuzzleLibrary).
/// difficultyLevel (1-10) is that language's original level number - each of the 10 scenes
/// per language (Assets/Main/Scenes/3 Crossword/&lt;N&gt; - Puzzle - &lt;Language&gt;) is set to a
/// fixed difficulty matching its own scene number (scene 1 = level 1, ... scene 10 = level
/// 10), so difficulty is deterministic per scene rather than random. useRandomLevel (off by
/// default) overrides this with a random level 1-10 each time - only the original demo scene
/// (CrossWord - Generate.unity) still uses this, since it isn't part of that 1-10 scene set.
/// CurrentPuzzle/CurrentPuzzleSourceName expose whichever puzzle actually got loaded, so
/// CrosswordAnswerKey can show it without duplicating this logic.
///
/// Controls: click a cell to select it (clicking the same cell again toggles Across/Down
/// if it belongs to both); type letters to fill the active word and auto-advance;
/// Backspace clears and steps back; arrow keys move the cursor; Space toggles direction;
/// Tab jumps to the next clue in number order. A correctly solved word is locked for good: its
/// letters can't be overwritten or erased, and typing a crossing word skips over those cells and
/// only fills the unsolved ones.
/// </summary>
public class CrosswordInputController : MonoBehaviour
{
    [SerializeField] private CrosswordGridGenerator gridGenerator;
    [SerializeField] private CrosswordCluePanel cluePanel;

    [Header("Puzzle Source")]
    [Tooltip("Which language's puzzle set (Assets/Main/Data/Crossword/Legacy levels/<Language>) this scene draws from. Ilonggo uses the Hiligaynon data files.")]
    [SerializeField] private PuzzleLanguage language = PuzzleLanguage.Tagalog;

    [Tooltip("The fixed difficulty (that language's original level number, 1-10) this scene loads. Ignored if Use Random Level is on.")]
    [Range(1, 10)]
    [SerializeField] private int difficultyLevel = 1;

    [Tooltip("If on, ignores Difficulty Level and picks a random level 1-10 every time this generates instead.")]
    [SerializeField] private bool useRandomLevel = false;

    private class WordInfo
    {
        public CrosswordWordEntry Entry;
        public List<(int row, int col)> Cells;
    }

    private Dictionary<(int row, int col), CrosswordCell> _cells;
    private List<WordInfo> _words;
    private readonly Dictionary<(int row, int col), int> _acrossAt = new Dictionary<(int row, int col), int>();
    private readonly Dictionary<(int row, int col), int> _downAt = new Dictionary<(int row, int col), int>();

    private (int row, int col) _selectedCell;
    private WordInfo _activeWord;

    /// <summary>Every cell belonging to a correctly solved word. Locked cells are permanent: typing
    /// and Backspace skip over them instead of overwriting, so a crossing word typed later can only
    /// fill the cells that are still unsolved. Only ever grows until the next Initialize().</summary>
    private readonly HashSet<(int row, int col)> _lockedCells = new HashSet<(int row, int col)>();

    // ---- Added for the redesigned crossword screen (CrosswordGameController). All optional: with nothing
    // subscribed and InputEnabled left on, the controller behaves exactly as before.

    /// <summary>Set false to ignore all typing and navigation (pause window, result card).</summary>
    public bool InputEnabled { get; set; } = true;

    /// <summary>Raised once the grid and clue lists of a puzzle are built.</summary>
    public event System.Action PuzzleReady;

    /// <summary>Raised whenever the active word changes (click, typing, arrows, clue list).</summary>
    public event System.Action<CrosswordWordEntry> ActiveClueChanged;

    /// <summary>Raised the moment a word is completed correctly.</summary>
    public event System.Action<CrosswordWordEntry> WordSolved;

    /// <summary>Raised once, when every word of the puzzle is solved.</summary>
    public event System.Action PuzzleCompleted;

    private readonly HashSet<WordInfo> _solvedWords = new HashSet<WordInfo>();
    private bool _completedRaised;

    /// <summary>All words of the loaded puzzle, ordered by clue number.</summary>
    public List<CrosswordWordEntry> Entries
    {
        get { return _words == null ? new List<CrosswordWordEntry>() : _words.Select(w => w.Entry).OrderBy(e => e.number).ToList(); }
    }

    public int SolvedCount { get { return _solvedWords.Count; } }
    public int TotalWords { get { return _words == null ? 0 : _words.Count; } }
    public bool Completed { get { return _completedRaised; } }
    public CrosswordWordEntry ActiveEntry { get { return _activeWord == null ? null : _activeWord.Entry; } }

    /// <summary>The puzzle actually loaded by the most recent GenerateRandomPuzzle() call.</summary>
    public CrosswordPuzzle CurrentPuzzle { get; private set; }

    /// <summary>The data class backing CurrentPuzzle, e.g. "Hiligaynon_Level3" - for display only.</summary>
    public string CurrentPuzzleSourceName { get; private set; }

    private CrosswordPuzzle overridePuzzle;
    private string overrideSourceName;

    /// <summary>
    /// Gives this controller a puzzle to play instead of one of the old numbered levels (used by the Crossword category play scene,
    /// CrosswordCategoryBootstrap). Call it before Start (an early Awake is fine).
    /// </summary>
    public void SetPuzzleOverride(CrosswordPuzzle puzzle, string sourceName)
    {
        overridePuzzle = puzzle;
        overrideSourceName = sourceName;
    }

    private void Start()
    {
        // Don't generate on the very first frame: CrosswordCluePanel's column layout measures
        // acrossListParent/downListParent's RectTransform.rect.width to size and word-wrap each
        // clue row, and that width is ultimately driven by the Canvas's own size, which tracks
        // the actual window/screen size. In a freshly launched build (especially a resizable
        // windowed one - see Player Settings) the window hasn't finished sizing itself to its
        // real dimensions on frame 0, so the canvas (and everything under it) can briefly report
        // a wrong, much-too-narrow width - which is exactly what makes clue text wrap one word
        // per line and rows run far taller than the panel. The Editor never shows this because a
        // scene already sitting open has a stable, already-correct canvas size. Waiting one frame
        // lets the window/canvas settle to its real size before anything measures it.
        StartCoroutine(GenerateAfterLayoutSettles());
    }

    private System.Collections.IEnumerator GenerateAfterLayoutSettles()
    {
        yield return null;
        GeneratePuzzleForScene();
    }

    /// <summary>Right-click this component (or its gear icon) in the Inspector and pick this
    /// to (re)build the grid and clue lists right now, in Edit mode - no need to press Play
    /// just to see the puzzle on screen. Loads the same level Start() would (see class doc
    /// comment for how difficultyLevel/useRandomLevel decide which one that is).</summary>
    [ContextMenu("Generate Puzzle In Editor")]
    private void GenerateInEditor()
    {
        GeneratePuzzleForScene();
    }

    /// <summary>Resolves which level to load - difficultyLevel, or a random 1-10 if
    /// useRandomLevel is on - builds it, and pushes the result to CrosswordAnswerKey (if one
    /// exists in the scene) so its Inspector display stays in sync with whichever puzzle
    /// actually got loaded.</summary>
    private void GeneratePuzzleForScene()
    {
        CrosswordPuzzle puzzle;
        if (overridePuzzle != null)
        {
            // category play scene: the puzzle was handed over by CrosswordCategoryBootstrap
            puzzle = overridePuzzle;
            CurrentPuzzleSourceName = overrideSourceName;
        }
        else
        {
            int level = useRandomLevel ? Random.Range(1, 11) : difficultyLevel;
            puzzle = CrosswordPuzzleLibrary.Get(language, level);
            CurrentPuzzleSourceName = CrosswordPuzzleLibrary.GetSourceName(language, level);
        }
        Initialize(puzzle);

        CrosswordAnswerKey answerKey = FindObjectOfType<CrosswordAnswerKey>();
        if (answerKey != null)
        {
            answerKey.RefreshFrom(puzzle, CurrentPuzzleSourceName);
        }
    }

    public void Initialize(CrosswordPuzzle puzzle)
    {
        CurrentPuzzle = puzzle;

        // _activeWord (and its cached cell positions) belongs to whatever puzzle was loaded
        // before this call, if any. Clearing it here - before gridGenerator rebuilds Cells -
        // stops SelectWord()'s "un-highlight the previous active word" step from looking up
        // stale positions in the new grid, which don't exist there and throw
        // KeyNotFoundException. Without this, calling Initialize() a second time (exactly
        // what picking a new random level does) always crashes.
        _activeWord = null;
        _lockedCells.Clear();
        _solvedWords.Clear();
        _completedRaised = false;

        gridGenerator.Generate(puzzle);
        _cells = gridGenerator.Cells;
        BuildWordIndex(puzzle);

        foreach (CrosswordCell cell in _cells.Values)
        {
            cell.OnClicked = HandleCellClicked;
        }

        cluePanel.Initialize(_words.Select(w => w.Entry), SelectWordFromClue);

        if (_words.Count > 0)
        {
            SelectWord(_words[0], _words[0].Cells[0]);
        }

        if (PuzzleReady != null)
        {
            PuzzleReady();
        }
    }

    private void BuildWordIndex(CrosswordPuzzle puzzle)
    {
        _words = new List<WordInfo>();
        _acrossAt.Clear();
        _downAt.Clear();

        foreach (CrosswordWordEntry entry in puzzle.words)
        {
            var cells = new List<(int row, int col)>();
            for (int i = 0; i < entry.answer.Length; i++)
            {
                int r = entry.direction == CrosswordDirection.Down ? entry.row + i : entry.row;
                int c = entry.direction == CrosswordDirection.Across ? entry.col + i : entry.col;
                cells.Add((r, c));
            }

            _words.Add(new WordInfo { Entry = entry, Cells = cells });
            int index = _words.Count - 1;
            Dictionary<(int row, int col), int> map = entry.direction == CrosswordDirection.Across ? _acrossAt : _downAt;
            foreach ((int row, int col) pos in cells)
            {
                map[pos] = index;
            }
        }
    }

    private void HandleCellClicked(CrosswordCell cell)
    {
        var pos = (cell.Row, cell.Col);
        CrosswordDirection direction = _activeWord?.Entry.direction ?? CrosswordDirection.Across;

        bool sameCell = _activeWord != null && _selectedCell == pos;
        if (sameCell && _acrossAt.ContainsKey(pos) && _downAt.ContainsKey(pos))
        {
            direction = direction == CrosswordDirection.Across ? CrosswordDirection.Down : CrosswordDirection.Across;
        }
        else if (!sameCell)
        {
            bool hasInCurrentDirection = (direction == CrosswordDirection.Across ? _acrossAt : _downAt).ContainsKey(pos);
            if (!hasInCurrentDirection)
            {
                direction = _acrossAt.ContainsKey(pos) ? CrosswordDirection.Across : CrosswordDirection.Down;
            }
        }

        SelectCellInDirection(pos, direction);
    }

    private void SelectCellInDirection((int row, int col) pos, CrosswordDirection direction)
    {
        Dictionary<(int row, int col), int> map = direction == CrosswordDirection.Across ? _acrossAt : _downAt;
        if (!map.TryGetValue(pos, out int wordIndex))
        {
            direction = direction == CrosswordDirection.Across ? CrosswordDirection.Down : CrosswordDirection.Across;
            map = direction == CrosswordDirection.Across ? _acrossAt : _downAt;
            if (!map.TryGetValue(pos, out wordIndex))
            {
                return;
            }
        }

        SelectWord(_words[wordIndex], pos);
    }

    private void SelectWord(WordInfo word, (int row, int col) selectedPos)
    {
        if (_activeWord != null)
        {
            foreach ((int row, int col) p in _activeWord.Cells)
            {
                _cells[p].SetState(CrosswordCell.VisualState.Normal);
            }
        }

        _activeWord = word;
        _selectedCell = selectedPos;

        foreach ((int row, int col) p in word.Cells)
        {
            _cells[p].SetState(p == selectedPos ? CrosswordCell.VisualState.ActiveCell : CrosswordCell.VisualState.ActiveWord);
        }

        cluePanel.SetActiveClue(word.Entry);

        if (ActiveClueChanged != null)
        {
            ActiveClueChanged(word.Entry);
        }
    }

    private void SelectWordFromClue(CrosswordWordEntry entry)
    {
        WordInfo word = _words.First(w => w.Entry == entry);
        SelectWord(word, word.Cells[FirstUnlockedIndex(word)]);
    }

    /// <summary>Index of the first cell in word that isn't locked, or 0 if the whole word is solved.</summary>
    private int FirstUnlockedIndex(WordInfo word)
    {
        int index = NextUnlockedIndex(word, 0);
        return index >= 0 ? index : 0;
    }

    /// <summary>Index of the first unlocked cell in word at or after fromIndex, wrapping around to the
    /// start of the word if none follows; -1 when every cell of the word is locked.</summary>
    private int NextUnlockedIndex(WordInfo word, int fromIndex)
    {
        for (int i = 0; i < word.Cells.Count; i++)
        {
            int index = (fromIndex + i) % word.Cells.Count;
            if (!_lockedCells.Contains(word.Cells[index]))
            {
                return index;
            }
        }
        return -1;
    }

    private void Update()
    {
        if (_activeWord == null || !InputEnabled)
        {
            return;
        }

        string input = Input.inputString;
        if (!string.IsNullOrEmpty(input))
        {
            foreach (char raw in input)
            {
                if (raw == '\b')
                {
                    Backspace();
                }
                else
                {
                    char c = char.ToUpperInvariant(raw);
                    if (c >= 'A' && c <= 'Z')
                    {
                        TypeLetter(c);
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow)) MoveSelection(0, -1);
        if (Input.GetKeyDown(KeyCode.RightArrow)) MoveSelection(0, 1);
        if (Input.GetKeyDown(KeyCode.UpArrow)) MoveSelection(-1, 0);
        if (Input.GetKeyDown(KeyCode.DownArrow)) MoveSelection(1, 0);
        if (Input.GetKeyDown(KeyCode.Space)) ToggleDirectionAtSelection();
        if (Input.GetKeyDown(KeyCode.Tab)) SelectNextClue();
    }

    private void TypeLetter(char c)
    {
        int index = _activeWord.Cells.IndexOf(_selectedCell);
        if (index < 0)
        {
            return;
        }

        // Solved cells are permanent: if the cursor sits on one, the letter goes into the next
        // unsolved cell of this word instead (nothing happens once the whole word is solved).
        if (_lockedCells.Contains(_selectedCell))
        {
            index = NextUnlockedIndex(_activeWord, index);
            if (index < 0)
            {
                return;
            }
            SelectWord(_activeWord, _activeWord.Cells[index]);
        }

        _cells[_selectedCell].SetLetter(c);
        RefreshCompletionStates();

        for (int i = index + 1; i < _activeWord.Cells.Count; i++)
        {
            if (!_lockedCells.Contains(_activeWord.Cells[i]))
            {
                SelectWord(_activeWord, _activeWord.Cells[i]);
                return;
            }
        }
    }

    private void Backspace()
    {
        if (!_lockedCells.Contains(_selectedCell) && _cells[_selectedCell].CurrentLetter != '\0')
        {
            _cells[_selectedCell].SetLetter('\0');
            RefreshCompletionStates();
            return;
        }

        int index = _activeWord.Cells.IndexOf(_selectedCell);
        for (int i = index - 1; i >= 0; i--)
        {
            (int row, int col) prev = _activeWord.Cells[i];
            if (_lockedCells.Contains(prev))
            {
                continue;
            }
            SelectWord(_activeWord, prev);
            _cells[prev].SetLetter('\0');
            RefreshCompletionStates();
            return;
        }
    }

    private void MoveSelection(int deltaRow, int deltaCol)
    {
        var next = (_selectedCell.row + deltaRow, _selectedCell.col + deltaCol);
        if (!_cells.ContainsKey(next))
        {
            return;
        }
        SelectCellInDirection(next, _activeWord.Entry.direction);
    }

    private void ToggleDirectionAtSelection()
    {
        if (_acrossAt.ContainsKey(_selectedCell) && _downAt.ContainsKey(_selectedCell))
        {
            CrosswordDirection newDirection = _activeWord.Entry.direction == CrosswordDirection.Across ? CrosswordDirection.Down : CrosswordDirection.Across;
            SelectCellInDirection(_selectedCell, newDirection);
        }
    }

    private void SelectNextClue()
    {
        int currentIndex = _words.IndexOf(_activeWord);
        int nextIndex = (currentIndex + 1) % _words.Count;
        SelectWord(_words[nextIndex], _words[nextIndex].Cells[FirstUnlockedIndex(_words[nextIndex])]);
    }

    private void RefreshCompletionStates()
    {
        var newlySolved = new List<WordInfo>();
        foreach (WordInfo word in _words)
        {
            bool complete = word.Cells.All(p => _cells[p].IsCorrect);
            cluePanel.SetCompleted(word.Entry, complete);
            if (complete)
            {
                _lockedCells.UnionWith(word.Cells);
                foreach ((int row, int col) p in word.Cells)
                {
                    _cells[p].SetSolved(true);
                }
                if (_solvedWords.Add(word))
                {
                    newlySolved.Add(word);
                }
            }
        }

        foreach (WordInfo word in newlySolved)
        {
            if (WordSolved != null)
            {
                WordSolved(word.Entry);
            }
        }

        if (!_completedRaised && _words.Count > 0 && _solvedWords.Count == _words.Count)
        {
            _completedRaised = true;
            if (PuzzleCompleted != null)
            {
                PuzzleCompleted();
            }
        }
    }

    // ---------------------------------------------------------------- UI entry points (on-screen keypad and tools)

    /// <summary>Types one letter into the active word, like pressing a key (on-screen keypad).</summary>
    public void TypeLetterFromUi(char c)
    {
        if (_activeWord == null || !InputEnabled)
        {
            return;
        }
        c = char.ToUpperInvariant(c);
        if (c >= 'A' && c <= 'Z')
        {
            TypeLetter(c);
        }
    }

    /// <summary>Deletes a letter, like pressing Backspace (on-screen keypad).</summary>
    public void BackspaceFromUi()
    {
        if (_activeWord == null || !InputEnabled)
        {
            return;
        }
        Backspace();
    }

    /// <summary>Jumps to the next clue in number order.</summary>
    public void GoToNextClue()
    {
        if (_activeWord == null || !InputEnabled)
        {
            return;
        }
        SelectNextClue();
    }

    /// <summary>Jumps to the previous clue in number order.</summary>
    public void GoToPreviousClue()
    {
        if (_activeWord == null || !InputEnabled)
        {
            return;
        }
        int currentIndex = _words.IndexOf(_activeWord);
        int previousIndex = (currentIndex - 1 + _words.Count) % _words.Count;
        SelectWord(_words[previousIndex], _words[previousIndex].Cells[FirstUnlockedIndex(_words[previousIndex])]);
    }

    /// <summary>Hint: fills in one correct letter of the active word (the first one that is empty or wrong). Returns false if the word has nothing left to reveal.</summary>
    public bool RevealOneLetter()
    {
        if (_activeWord == null || !InputEnabled)
        {
            return false;
        }
        foreach ((int row, int col) p in _activeWord.Cells)
        {
            if (_lockedCells.Contains(p) || _cells[p].IsCorrect)
            {
                continue;
            }
            SelectWord(_activeWord, p);
            TypeLetter(_cells[p].CorrectLetter);
            return true;
        }
        return false;
    }

    /// <summary>Fills in the whole active word with its correct letters. Returns false if it was already solved.</summary>
    public bool RevealActiveWord()
    {
        if (_activeWord == null || !InputEnabled)
        {
            return false;
        }
        bool changed = false;
        foreach ((int row, int col) p in _activeWord.Cells)
        {
            if (!_lockedCells.Contains(p) && !_cells[p].IsCorrect)
            {
                _cells[p].SetLetter(_cells[p].CorrectLetter);
                changed = true;
            }
        }
        if (changed)
        {
            RefreshCompletionStates();
        }
        return changed;
    }

    /// <summary>Check: marks the typed letters of the active word that are wrong. Returns how many were marked.</summary>
    public int CheckActiveWord()
    {
        if (_activeWord == null)
        {
            return 0;
        }
        int wrong = 0;
        foreach ((int row, int col) p in _activeWord.Cells)
        {
            CrosswordCell cell = _cells[p];
            if (!_lockedCells.Contains(p) && cell.CurrentLetter != '\0' && !cell.IsCorrect)
            {
                cell.SetWrong(true);
                wrong++;
            }
        }
        return wrong;
    }

    /// <summary>Removes the red "wrong" marks left by CheckActiveWord.</summary>
    public void ClearWrongMarks()
    {
        if (_cells == null)
        {
            return;
        }
        foreach (CrosswordCell cell in _cells.Values)
        {
            if (cell != null)
            {
                cell.SetWrong(false);
            }
        }
    }
}
