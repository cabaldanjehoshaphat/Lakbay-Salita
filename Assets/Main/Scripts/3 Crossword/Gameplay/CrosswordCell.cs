using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// CrosswordCell
/// One letter cell in the crossword grid, instantiated by CrosswordGridGenerator - one
/// per grid position that a word's letters actually cover (black/blocked cells use a
/// separate, non-interactive prefab instead of this one). Holds the correct letter for
/// this position and whatever the player has typed so far, and reports clicks upward via
/// OnClicked rather than managing its own selection state - CrosswordInputController
/// decides what a click means (which word becomes active, which direction) since that
/// depends on the whole puzzle, not just this one cell.
///
/// Visually the cell is two stacked Images: the root's own Image is the border (full cell
/// size, tinted by CrosswordTextStyle.borderColor/borderOpacity), and a "Fill" child inset
/// a few pixels inside it is the actual state color (white/highlighted/active) - so the
/// border is just whatever thin ring of the root shows around the inset fill, with no
/// separate border sprite needed.
/// </summary>
public class CrosswordCell : MonoBehaviour, IPointerClickHandler
{
    public enum VisualState
    {
        Normal,
        ActiveWord,
        ActiveCell
    }

    [SerializeField] private Image border;
    [SerializeField] private Image fill;
    [SerializeField] private RectTransform fillRect;
    [SerializeField] private TMP_Text letterText;
    [SerializeField] private TMP_Text numberText;

    [Tooltip("Fill color when this cell is not part of the active word.")]
    public Color normalColor = Color.white;

    [Tooltip("Fill color for every cell in the active word (except the current cell).")]
    public Color activeWordColor = new Color(0.72f, 0.85f, 0.98f);

    [Tooltip("Fill color for the exact cell the player's cursor is on.")]
    public Color activeCellColor = new Color(1f, 0.85f, 0.3f);

    [Tooltip("Fill color of a cell that belongs to a correctly answered (finished) word - stays green whether or not the word is highlighted.")]
    public Color solvedColor = new Color(0.55f, 0.86f, 0.5f);

    [Tooltip("Fill color of a solved cell while the player's cursor is on it (a deeper green so the cursor is still visible).")]
    public Color solvedActiveCellColor = new Color(0.32f, 0.72f, 0.3f);

    [Header("Optional extras (used by the redesigned crossword screen)")]
    [Tooltip("When on, the cell the cursor is on gets cursorBorderColor on its border instead of relying on the fill colour alone.")]
    [SerializeField] private bool cursorShowsBorder = false;

    [SerializeField] private Color cursorBorderColor = new Color(0.96f, 0.65f, 0.14f);

    [Tooltip("Fill colour of a typed letter that the Check button found to be wrong. Clears as soon as the letter changes.")]
    public Color wrongColor = new Color(0.97f, 0.78f, 0.78f);

    private Color _styleBorderColor = Color.black;
    private bool _hasStyleBorder;
    private bool _wrong;

    public int Row { get; private set; }
    public int Col { get; private set; }
    public char CorrectLetter { get; private set; }
    public char CurrentLetter => letterText.text.Length > 0 ? letterText.text[0] : '\0';
    public bool IsCorrect => CurrentLetter == CorrectLetter;

    /// <summary>True once a word containing this cell has been answered correctly; the cell then shows the solved (green) color.</summary>
    public bool IsSolved { get; private set; }

    private VisualState _state = VisualState.Normal;

    /// <summary>Assigned by CrosswordInputController after the grid is built.</summary>
    public Action<CrosswordCell> OnClicked;

    /// <summary>Sets this cell's grid position, correct letter, and clue number (null if it doesn't start a clue), and resets its visuals.</summary>
    public void Initialize(int row, int col, char correctLetter, int? clueNumber)
    {
        Row = row;
        Col = col;
        CorrectLetter = correctLetter;
        numberText.text = clueNumber.HasValue ? clueNumber.Value.ToString() : string.Empty;
        letterText.text = string.Empty;
        IsSolved = false;
        _wrong = false;
        SetState(VisualState.Normal);
    }

    /// <summary>Marks (or clears) this cell as holding a wrong letter, as found by the Check button.</summary>
    public void SetWrong(bool wrong)
    {
        _wrong = wrong;
        SetState(_state);
    }

    /// <summary>Marks this cell as part of a finished word (or clears that), re-applying the current highlight so the green shows immediately.</summary>
    public void SetSolved(bool solved)
    {
        IsSolved = solved;
        SetState(_state);
    }

    public void SetLetter(char c)
    {
        letterText.text = c == '\0' ? string.Empty : c.ToString();
        if (_wrong)
        {
            _wrong = false;
            SetState(_state);
        }
    }

    public void SetState(VisualState state)
    {
        _state = state;
        if (fill == null)
        {
            return;
        }
        if (IsSolved)
        {
            fill.color = state == VisualState.ActiveCell ? solvedActiveCellColor : solvedColor;
            ApplyCursorBorder(state);
            return;
        }
        if (_wrong)
        {
            fill.color = wrongColor;
            ApplyCursorBorder(state);
            return;
        }
        fill.color = state switch
        {
            VisualState.ActiveCell => activeCellColor,
            VisualState.ActiveWord => activeWordColor,
            _ => normalColor,
        };
        ApplyCursorBorder(state);
    }

    private void ApplyCursorBorder(VisualState state)
    {
        if (!cursorShowsBorder || border == null || !_hasStyleBorder)
        {
            return;
        }
        border.color = (state == VisualState.ActiveCell && !IsSolved) ? cursorBorderColor : _styleBorderColor;
    }

    /// <summary>Applies shared letter/number/border settings from the one CrosswordTextStyle asset. Safe to call repeatedly (e.g. live from the Editor).</summary>
    public void ApplyStyle(CrosswordTextStyle style)
    {
        if (style == null)
        {
            return;
        }

        letterText.fontSize = style.letterFontSize;
        letterText.fontStyle = style.letterFontStyle;
        letterText.color = style.letterColor;

        numberText.fontSize = style.cellNumberFontSize;
        numberText.color = style.cellNumberColor;

        if (border != null)
        {
            Color c = style.borderColor;
            c.a = style.borderOpacity;
            border.color = c;
            _styleBorderColor = c;
            _hasStyleBorder = true;
            ApplyCursorBorder(_state);
        }

        if (fillRect != null)
        {
            fillRect.offsetMin = new Vector2(style.borderThickness, style.borderThickness);
            fillRect.offsetMax = new Vector2(-style.borderThickness, -style.borderThickness);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClicked?.Invoke(this);
    }
}
