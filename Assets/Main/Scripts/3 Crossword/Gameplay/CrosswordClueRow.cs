using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// CrosswordClueRow
/// One clickable row in the Across/Down clue lists, instantiated by CrosswordCluePanel -
/// one per CrosswordWordEntry. Shows the clue number and text, highlights itself when its
/// word is the active one, and strikes its text through once that word is fully and
/// correctly filled in. Clicking it asks CrosswordInputController (via the callback passed
/// into Initialize) to select this word's start cell.
/// </summary>
public class CrosswordClueRow : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TMP_Text numberText;
    [SerializeField] private TMP_Text clueText;
    [SerializeField] private Image background;

    [Tooltip("Background color when this row's word is not the active one.")]
    public Color normalColor = new Color(0f, 0f, 0f, 0f);

    [Tooltip("Background color when this row's word is the active one.")]
    public Color activeColor = new Color(0.72f, 0.85f, 0.98f);

    [Tooltip("Clue text color once the word is fully and correctly filled in.")]
    public Color completedColor = new Color(0.6f, 0.6f, 0.6f);

    [Tooltip("Extra vertical space added above/below the clue text when sizing this row to fit wrapped text.")]
    public float verticalPadding = 6f;

    [Header("Optional extras (used by the redesigned crossword screen)")]
    [Tooltip("Background colour of a finished (solved) row. Leave fully transparent to keep the original look.")]
    public Color completedBackgroundColor = new Color(0f, 0f, 0f, 0f);

    [Tooltip("Optional object (a check mark) shown only while this row's word is solved.")]
    [SerializeField] private GameObject completedBadge;

    private bool _isActive;
    private bool _isCompleted;

    [Header("Text Size (read-only)")]
    [Tooltip("The CrosswordTextStyle asset currently driving this row's text size/color - the SAME single asset used by every cell and every clue row in the crossword. To change text size, edit THIS asset (double-click it here to select it), not this row.")]
    [SerializeField] private CrosswordTextStyle appliedStyle;

    private Color _normalTextColor;
    private RectTransform _rootRect;

    public CrosswordWordEntry Entry { get; private set; }
    private Action _onClick;

    public void Initialize(CrosswordWordEntry entry, Action onClick)
    {
        Entry = entry;
        _onClick = onClick;
        numberText.text = entry.number.ToString();
        clueText.text = entry.clue;
        clueText.enableWordWrapping = true;
        clueText.overflowMode = TMPro.TextOverflowModes.Overflow;
        _normalTextColor = clueText.color;
        SetActive(false);
        SetCompleted(false);
    }

    /// <summary>Resizes this row's own RectTransform to fit its (possibly wrapped) clue text,
    /// so CrosswordCluePanel can stack rows without overlap regardless of clueTextFontSize.
    /// Returns the resulting row height.</summary>
    public float RefreshHeight()
    {
        if (_rootRect == null)
        {
            _rootRect = GetComponent<RectTransform>();
        }
        if (clueText == null)
        {
            return _rootRect.sizeDelta.y;
        }

        float width = clueText.rectTransform.rect.width;
        float textHeight = clueText.GetPreferredValues(clueText.text, width, 0f).y;
        float numberHeight = numberText != null ? numberText.preferredHeight : 0f;
        float height = Mathf.Max(textHeight, numberHeight) + verticalPadding;

        _rootRect.sizeDelta = new Vector2(_rootRect.sizeDelta.x, height);
        return height;
    }

    public void SetActive(bool active)
    {
        _isActive = active;
        UpdateBackground();
    }

    public void SetCompleted(bool completed)
    {
        _isCompleted = completed;
        clueText.fontStyle = completed ? FontStyles.Strikethrough : FontStyles.Normal;
        clueText.color = completed ? completedColor : _normalTextColor;
        if (completedBadge != null)
        {
            completedBadge.SetActive(completed);
        }
        UpdateBackground();
    }

    private void UpdateBackground()
    {
        if (background == null)
        {
            return;
        }
        if (_isActive)
        {
            background.color = activeColor;
        }
        else if (_isCompleted && completedBackgroundColor.a > 0f)
        {
            background.color = completedBackgroundColor;
        }
        else
        {
            background.color = normalColor;
        }
    }

    /// <summary>Applies shared number/clue text size and color from the one CrosswordTextStyle asset.</summary>
    public void ApplyStyle(CrosswordTextStyle style)
    {
        if (style == null)
        {
            return;
        }

        appliedStyle = style;

        numberText.fontSize = style.clueNumberFontSize;
        numberText.color = style.clueNumberColor;

        clueText.fontSize = style.clueTextFontSize;
        _normalTextColor = style.clueTextColor;
        if (clueText.fontStyle != FontStyles.Strikethrough)
        {
            clueText.color = _normalTextColor;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _onClick?.Invoke();
    }
}
