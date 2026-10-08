using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// CrosswordGridGenerator
/// Builds the visible crossword grid for one CrosswordPuzzle (see CrosswordPuzzleData.cs):
/// instantiates a CrosswordCell for every grid position a word's letters actually cover,
/// and a plain BlackCell for every other position inside the puzzle's bounding box - so
/// black cells are derived from the word list rather than authored by hand. Does not run
/// itself on Start(); CrosswordInputController calls Generate() so it can wire up input
/// handling and the clue panel against the resulting cells immediately afterward.
///
/// cellSize is the MAXIMUM size a cell is drawn at - spacing is the gap kept between cells
/// regardless of size. Different puzzles (this scene may load any random level for its
/// language, see CrosswordInputController) span very different row/col counts, so every
/// reflow first checks whether the grid at cellSize would fit inside gridParent's own
/// rect (times fillFraction, so it doesn't touch the edges) and shrinks cells uniformly
/// just enough to fit if not - a puzzle wider or taller than the panel no longer overflows
/// it, it just renders with smaller cells. Editing cellSize, spacing or textStyle in the
/// Inspector (or editing textStyle's own asset - the SAME asset CrosswordCluePanel uses, so
/// one place controls every text size in the crossword) re-lays-out whatever grid is
/// already on screen immediately via OnValidate - no need to re-enter Play mode to see the
/// effect.
/// </summary>
public class CrosswordGridGenerator : MonoBehaviour
{
    [SerializeField] private RectTransform gridParent;
    [SerializeField] private CrosswordCell cellPrefab;
    [SerializeField] private RectTransform blackCellPrefab;

    [Tooltip("The largest a cell is ever drawn, in pixels. Cells shrink below this (uniformly, keeping the grid square) only if the puzzle is too big to fit gridParent at this size.")]
    [SerializeField] private float cellSize = 80f;
    [SerializeField] private float spacing = 4f;
    [SerializeField] private CrosswordTextStyle textStyle;

    [Tooltip("Fraction of gridParent's own width/height the grid is allowed to fill before cells start shrinking to fit - leaves a margin so a full-size grid doesn't touch the panel's edges.")]
    [Range(0.5f, 1f)]
    [SerializeField] private float fillFraction = 0.92f;

    [Tooltip("Shifts the whole grid within gridParent: X = left/right, Y = top/bottom. (0,0) centers it.")]
    [SerializeField] private Vector2 gridOffset = Vector2.zero;

    public CrosswordPuzzle Puzzle { get; private set; }
    public Dictionary<(int row, int col), CrosswordCell> Cells { get; } = new Dictionary<(int row, int col), CrosswordCell>();
    private readonly List<CrosswordBlackCell> _blackCells = new List<CrosswordBlackCell>();

    /// <summary>Which style instance ApplyStyleToAllCells is currently subscribed to - lets
    /// EnsureSubscribedToTextStyle() tell "already subscribed" apart from "need to (re)wire
    /// this up", including after a script recompile, which silently drops this subscription
    /// (event subscriptions aren't serialized, so a domain reload always clears them - see
    /// EnsureSubscribedToTextStyle for why OnEnable alone isn't reliable enough to restore it).</summary>
    private CrosswordTextStyle _subscribedStyle;

    private void OnEnable()
    {
        EnsureSubscribedToTextStyle();
    }

    private void OnDisable()
    {
        if (_subscribedStyle != null)
        {
            _subscribedStyle.Changed -= ApplyStyleToAllCells;
            _subscribedStyle = null;
        }
    }

    /// <summary>Makes sure ApplyStyleToAllCells is subscribed to textStyle.Changed, no matter
    /// when this is called. OnEnable alone isn't reliable for this: after a script recompile,
    /// scene objects and ScriptableObject assets can finish reloading in a different order,
    /// so an OnEnable-only subscription can silently attach to a textStyle instance that gets
    /// replaced moments later - leaving Changed with zero subscribers even though everything
    /// LOOKS wired up in the Inspector. Calling this again from OnValidate and Generate() as
    /// well means the subscription self-heals the next time this component does anything,
    /// instead of staying silently broken until the next domain reload.</summary>
    private void EnsureSubscribedToTextStyle()
    {
        if (_subscribedStyle == textStyle)
        {
            return;
        }
        if (_subscribedStyle != null)
        {
            _subscribedStyle.Changed -= ApplyStyleToAllCells;
        }
        _subscribedStyle = textStyle;
        if (_subscribedStyle != null)
        {
            _subscribedStyle.Changed += ApplyStyleToAllCells;
        }
    }

    /// <summary>Unity calls this whenever a field above is edited in the Inspector. If a
    /// grid is already built, re-lay it out with the new cellSize/spacing immediately
    /// instead of waiting for the next Generate() call.</summary>
    private void OnValidate()
    {
        EnsureSubscribedToTextStyle();
        ReflowLayout();
    }

    public void Generate(CrosswordPuzzle puzzle)
    {
        EnsureSubscribedToTextStyle();

        for (int i = gridParent.childCount - 1; i >= 0; i--)
        {
            GameObject child = gridParent.GetChild(i).gameObject;
            if (Application.isPlaying)
            {
                Destroy(child);
            }
            else
            {
                DestroyImmediate(child);
            }
        }
        Cells.Clear();
        _blackCells.Clear();
        Puzzle = puzzle;

        var numberByCell = new Dictionary<(int row, int col), int>();
        var letterByCell = new Dictionary<(int row, int col), char>();
        foreach (CrosswordWordEntry word in puzzle.words)
        {
            numberByCell[(word.row, word.col)] = word.number;
            for (int i = 0; i < word.answer.Length; i++)
            {
                int r = word.direction == CrosswordDirection.Down ? word.row + i : word.row;
                int c = word.direction == CrosswordDirection.Across ? word.col + i : word.col;
                letterByCell[(r, c)] = word.answer[i];
            }
        }

        (int minRow, int maxRow, int minCol, int maxCol) = Bounds(puzzle);

        for (int r = minRow; r <= maxRow; r++)
        {
            for (int c = minCol; c <= maxCol; c++)
            {
                if (letterByCell.TryGetValue((r, c), out char correctLetter))
                {
                    CrosswordCell cell = Instantiate(cellPrefab, gridParent);
                    int? number = numberByCell.TryGetValue((r, c), out int n) ? n : (int?)null;
                    cell.Initialize(r, c, correctLetter, number);
                    cell.ApplyStyle(textStyle);
                    Cells[(r, c)] = cell;
                }
                else
                {
                    RectTransform black = Instantiate(blackCellPrefab, gridParent);
                    CrosswordBlackCell marker = black.gameObject.AddComponent<CrosswordBlackCell>();
                    marker.Initialize(r, c);
                    _blackCells.Add(marker);
                }
            }
        }

        ReflowLayout();
    }

    /// <summary>Re-applies textStyle to every cell already on screen, without rebuilding the
    /// grid. Reads cells straight from gridParent's children instead of the Cells dictionary:
    /// Cells is runtime-only state that Unity clears on any reload (script recompile OR
    /// stopping Play mode) while the actual cell GameObjects stay put, so scanning the real
    /// hierarchy here keeps a style edit working right after one of those reloads instead of
    /// updating an empty dictionary while the grid on screen sits unchanged.</summary>
    public void ApplyStyleToAllCells()
    {
        if (gridParent == null)
        {
            return;
        }
        for (int i = 0; i < gridParent.childCount; i++)
        {
            CrosswordCell cell = gridParent.GetChild(i).GetComponent<CrosswordCell>();
            if (cell != null)
            {
                cell.ApplyStyle(textStyle);
            }
        }
    }

    /// <summary>Recomputes every cell's (and black cell's) position and size from the
    /// current cellSize/spacing (auto-shrunk to fit gridParent if needed - see the class
    /// doc comment), using each one's already-known row/col - no puzzle re-parsing needed,
    /// so this is cheap enough to call from OnValidate.</summary>
    private void ReflowLayout()
    {
        if (Puzzle == null || (Cells.Count == 0 && _blackCells.Count == 0))
        {
            return;
        }

        (int minRow, int maxRow, int minCol, int maxCol) = Bounds(Puzzle);
        int cols = maxCol - minCol + 1;
        int rows = maxRow - minRow + 1;

        float step = cellSize + spacing;
        if (gridParent != null && cols > 0 && rows > 0)
        {
            float availableWidth = gridParent.rect.width * fillFraction;
            float availableHeight = gridParent.rect.height * fillFraction;
            float maxStepForWidth = availableWidth / cols;
            float maxStepForHeight = availableHeight / rows;
            step = Mathf.Min(step, maxStepForWidth, maxStepForHeight);
        }
        float effectiveCellSize = Mathf.Max(1f, step - spacing);

        float startX = -(cols - 1) * step / 2f + gridOffset.x;
        float startY = (rows - 1) * step / 2f + gridOffset.y;
        var cellSizeVector = new Vector2(effectiveCellSize, effectiveCellSize);

        foreach (CrosswordCell cell in Cells.Values)
        {
            if (cell == null) continue;
            RectTransform rect = cell.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(startX + (cell.Col - minCol) * step, startY - (cell.Row - minRow) * step);
            rect.sizeDelta = cellSizeVector;
        }

        foreach (CrosswordBlackCell black in _blackCells)
        {
            if (black == null) continue;
            RectTransform rect = black.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(startX + (black.Col - minCol) * step, startY - (black.Row - minRow) * step);
            rect.sizeDelta = cellSizeVector;
        }
    }

    private static (int minRow, int maxRow, int minCol, int maxCol) Bounds(CrosswordPuzzle puzzle)
    {
        int minRow = puzzle.words.Min(w => w.row);
        int maxRow = puzzle.words.Max(w => w.direction == CrosswordDirection.Down ? w.row + w.answer.Length - 1 : w.row);
        int minCol = puzzle.words.Min(w => w.col);
        int maxCol = puzzle.words.Max(w => w.direction == CrosswordDirection.Across ? w.col + w.answer.Length - 1 : w.col);
        return (minRow, maxRow, minCol, maxCol);
    }
}
