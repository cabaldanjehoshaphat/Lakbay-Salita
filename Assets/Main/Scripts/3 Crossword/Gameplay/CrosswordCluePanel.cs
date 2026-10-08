using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

/// <summary>
/// CrosswordCluePanel
/// Builds and maintains the ACROSS/DOWN clue lists (one CrosswordClueRow per word, see
/// CrosswordClueRow.cs) plus the active-clue banner shown above the grid. Owned and driven
/// by CrosswordInputController: Initialize() populates the lists from the loaded puzzle,
/// SetActiveClue() re-highlights whichever word the player's cursor is currently in, and
/// SetCompleted() strikes a clue through once its word is fully and correctly filled.
///
/// acrossOffset/downOffset position each list's rows within their parent: X shifts every
/// row left/right together, Y sets where the first row sits (below whatever header/label
/// already lives in that parent). columnCount (1 or 2) lays the list out newspaper-style -
/// at 2, the first half of the rows fill column 1 top-to-bottom and the rest fill column 2
/// beside it, so a long clue list doesn't run off the bottom of the panel.
/// textStyle controls row text size/color and updates already-built rows live - it's the
/// SAME CrosswordTextStyle asset CrosswordGridGenerator uses for the grid, so one asset
/// controls every text size in the whole crossword, not a separate one per panel. Any style
/// change re-stacks the rows too, since a bigger font also makes each row taller.
/// </summary>
public class CrosswordCluePanel : MonoBehaviour
{
    [SerializeField] private RectTransform acrossListParent;
    [SerializeField] private RectTransform downListParent;
    [SerializeField] private CrosswordClueRow rowPrefab;
    [SerializeField] private TMP_Text activeClueNumberBadge;
    [SerializeField] private TMP_Text activeClueText;
    [Tooltip("Gap between rows, on top of each row's own height (which grows automatically to fit wrapped clue text).")]
    [SerializeField] private float rowSpacing = 6f;
    [SerializeField] private CrosswordTextStyle textStyle;

    [Header("Columns (Word-style newspaper layout)")]
    [Tooltip("1 = single list going straight down. 2 = fills the first column top-to-bottom, then continues in a second column beside it - keeps a long clue list from running off the bottom of the panel.")]
    [Range(1, 2)]
    [SerializeField] private int columnCount = 2;

    [Tooltip("Horizontal gap between column 1 and column 2 (only used when Column Count is 2).")]
    [SerializeField] private float columnGap = 16f;

    [Tooltip("Empty space kept between the clue text and the right edge of the Across/Down panel, in canvas pixels (48 = about 0.5 inch).")]
    [SerializeField] private float rightMargin = 48f;

    [Tooltip("Position (X = left/right, Y = top/bottom for the first row) of the Across list, in acrossListParent's local space.")]
    [SerializeField] private Vector2 acrossOffset = new Vector2(0f, -40f);

    [Tooltip("Position (X = left/right, Y = top/bottom for the first row) of the Down list, in downListParent's local space.")]
    [SerializeField] private Vector2 downOffset = new Vector2(0f, -40f);

    private readonly List<CrosswordClueRow> _rows = new List<CrosswordClueRow>();
    private Action<CrosswordWordEntry> _onClueSelected;

    /// <summary>Which style instance ApplyStyleToAllRows is currently subscribed to - see
    /// EnsureSubscribedToTextStyle for why OnEnable alone can't be trusted to keep this
    /// subscription alive across a script recompile.</summary>
    private CrosswordTextStyle _subscribedStyle;

    private void OnEnable()
    {
        EnsureSubscribedToTextStyle();
    }

    private void OnDisable()
    {
        if (_subscribedStyle != null)
        {
            _subscribedStyle.Changed -= ApplyStyleToAllRows;
            _subscribedStyle = null;
        }
    }

    /// <summary>Makes sure ApplyStyleToAllRows is subscribed to textStyle.Changed, no matter
    /// when this is called. OnEnable alone isn't reliable for this: after a script recompile,
    /// scene objects and ScriptableObject assets can finish reloading in a different order,
    /// so an OnEnable-only subscription can silently attach to a textStyle instance that gets
    /// replaced moments later - leaving Changed with zero subscribers even though everything
    /// LOOKS wired up in the Inspector (this is exactly what made "change the font size" stop
    /// updating the clue panel after editing these scripts). Calling this again from
    /// OnValidate and Initialize() means the subscription self-heals the next time this
    /// component does anything, instead of staying silently broken until the next reload.</summary>
    private void EnsureSubscribedToTextStyle()
    {
        if (_subscribedStyle == textStyle)
        {
            return;
        }
        if (_subscribedStyle != null)
        {
            _subscribedStyle.Changed -= ApplyStyleToAllRows;
        }
        _subscribedStyle = textStyle;
        if (_subscribedStyle != null)
        {
            _subscribedStyle.Changed += ApplyStyleToAllRows;
        }
    }

    /// <summary>Unity calls this whenever a field above is edited in the Inspector - reflow
    /// whatever rows already exist instead of waiting for the next Initialize(). Deferred by
    /// one editor tick because TMP's own layout callbacks can't run synchronously inside
    /// OnValidate (Unity logs "SendMessage cannot be called during ... OnValidate" if they try).</summary>
    private void OnValidate()
    {
        EnsureSubscribedToTextStyle();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.delayCall += DeferredReflow;
#else
        ReflowRows(acrossListParent, acrossOffset);
        ReflowRows(downListParent, downOffset);
#endif
    }

#if UNITY_EDITOR
    private void DeferredReflow()
    {
        if (this == null)
        {
            return;
        }
        ReflowRows(acrossListParent, acrossOffset);
        ReflowRows(downListParent, downOffset);
    }
#endif

    /// <summary>Lays out every CrosswordClueRow child of parent in up to 2 newspaper-style
    /// columns: column 1 fills top-to-bottom first, then column 2 starts back at the top
    /// beside it. Each row keeps its own auto-fit height (see CrosswordClueRow.RefreshHeight),
    /// so this re-measures and re-stacks from scratch every time - safe to call after any
    /// text-size change, not just after building the list.</summary>
    private void ReflowRows(RectTransform parent, Vector2 offset)
    {
        if (parent == null)
        {
            return;
        }

        var rows = new List<CrosswordClueRow>();
        for (int i = 0; i < parent.childCount; i++)
        {
            CrosswordClueRow row = parent.GetChild(i).GetComponent<CrosswordClueRow>();
            if (row != null)
            {
                rows.Add(row);
            }
        }
        if (rows.Count == 0)
        {
            return;
        }

        int columns = Mathf.Clamp(columnCount, 1, 2);
        int rowsPerColumn = Mathf.CeilToInt(rows.Count / (float)columns);

        float availableWidth = Mathf.Max(0f, parent.rect.width - offset.x - rightMargin);
        float columnWidth = columns > 1
            ? (availableWidth - columnGap * (columns - 1)) / columns
            : availableWidth;

        var cursorY = new float[columns];
        for (int c = 0; c < columns; c++)
        {
            cursorY[c] = offset.y;
        }

        for (int i = 0; i < rows.Count; i++)
        {
            int col = Mathf.Min(i / rowsPerColumn, columns - 1);
            CrosswordClueRow row = rows[i];
            RectTransform rect = row.GetComponent<RectTransform>();

            float x = offset.x + col * (columnWidth + columnGap);
            rect.sizeDelta = new Vector2(columnWidth, rect.sizeDelta.y);
            rect.anchoredPosition = new Vector2(x, cursorY[col]);

            float height = row.RefreshHeight();
            cursorY[col] -= height + rowSpacing;
        }
    }

    public void Initialize(IEnumerable<CrosswordWordEntry> words, Action<CrosswordWordEntry> onClueSelected)
    {
        EnsureSubscribedToTextStyle();
        _onClueSelected = onClueSelected;

        // Only remove previously-generated rows - acrossListParent/downListParent may also
        // hold a hand-placed header/label (e.g. "Across"/"Down" text) that must survive.
        RemoveExistingRows(acrossListParent);
        RemoveExistingRows(downListParent);
        _rows.Clear();

        List<CrosswordWordEntry> ordered = words.OrderBy(w => w.number).ToList();
        foreach (CrosswordWordEntry entry in ordered.Where(w => w.direction == CrosswordDirection.Across))
        {
            CreateRow(entry, acrossListParent);
        }
        foreach (CrosswordWordEntry entry in ordered.Where(w => w.direction == CrosswordDirection.Down))
        {
            CreateRow(entry, downListParent);
        }

        ReflowRows(acrossListParent, acrossOffset);
        ReflowRows(downListParent, downOffset);
    }

    private static void RemoveExistingRows(RectTransform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Transform child = parent.GetChild(i);
            if (child.GetComponent<CrosswordClueRow>() != null)
            {
                if (Application.isPlaying)
                {
                    // Destroy() only takes effect at the end of the frame, so detach first -
                    // otherwise ReflowRows (same frame) still counts these doomed rows and
                    // stacks the new rows underneath them, far below the panel's top. The
                    // Editor hid this via OnValidate's delayed reflow; a build has no such pass.
                    child.SetParent(null, false);
                    Destroy(child.gameObject);
                }
                else
                {
                    DestroyImmediate(child.gameObject);
                }
            }
        }
    }

    private void CreateRow(CrosswordWordEntry entry, RectTransform parent)
    {
        CrosswordClueRow row = Instantiate(rowPrefab, parent);
        row.Initialize(entry, () => _onClueSelected?.Invoke(entry));
        row.ApplyStyle(textStyle);
        _rows.Add(row);
    }

    /// <summary>Re-applies textStyle to every row already on screen, without rebuilding the
    /// list, then re-stacks both columns - a font size change resizes each row's own box, so
    /// skipping this reflow is what makes rows overlap/drift far out of place after editing
    /// the style asset. This runs from CrosswordTextStyle's own OnValidate (via its Changed
    /// event), so the reflow is deferred the same way CrosswordCluePanel.OnValidate defers it.
    ///
    /// Reads rows straight from acrossListParent/downListParent instead of the _rows list:
    /// _rows is runtime-only state, and Unity clears it (like every non-serialized field) on
    /// any reload - not just a script recompile, but also stopping Play mode - while the
    /// actual row GameObjects stay exactly where they were. Scanning the real hierarchy here
    /// means a style edit keeps working even right after one of those reloads, instead of
    /// silently updating an empty list while the rows on screen sit unchanged.</summary>
    public void ApplyStyleToAllRows()
    {
        foreach (CrosswordClueRow row in GetRowsInScene(acrossListParent))
        {
            row.ApplyStyle(textStyle);
        }
        foreach (CrosswordClueRow row in GetRowsInScene(downListParent))
        {
            row.ApplyStyle(textStyle);
        }
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.EditorApplication.delayCall += DeferredReflow;
            return;
        }
#endif
        ReflowRows(acrossListParent, acrossOffset);
        ReflowRows(downListParent, downOffset);
    }

    private static List<CrosswordClueRow> GetRowsInScene(RectTransform parent)
    {
        var rows = new List<CrosswordClueRow>();
        if (parent == null)
        {
            return rows;
        }
        for (int i = 0; i < parent.childCount; i++)
        {
            CrosswordClueRow row = parent.GetChild(i).GetComponent<CrosswordClueRow>();
            if (row != null)
            {
                rows.Add(row);
            }
        }
        return rows;
    }

    public void SetActiveClue(CrosswordWordEntry entry)
    {
        foreach (CrosswordClueRow row in _rows)
        {
            row.SetActive(row.Entry == entry);
        }

        if (activeClueNumberBadge != null)
        {
            activeClueNumberBadge.text = $"{entry.number} {entry.direction.ToString().ToUpperInvariant()}";
        }
        if (activeClueText != null)
        {
            activeClueText.text = entry.clue;
        }
    }

    public void SetCompleted(CrosswordWordEntry entry, bool completed)
    {
        CrosswordClueRow row = _rows.FirstOrDefault(r => r.Entry == entry);
        if (row != null)
        {
            row.SetCompleted(completed);
        }
    }
}
