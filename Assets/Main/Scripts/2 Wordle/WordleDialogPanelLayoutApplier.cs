using TMPro;
using UnityEngine;

// Applies a shared WordleDialogPanelLayout asset's left/right padding percentages to
// this Dialog Panel's RectTransform, keeping its vertical placement (bottom band)
// untouched — only the horizontal anchors change, so the panel stays symmetrically
// centered with equal padding on both sides, matching how its bottom edge already
// sits centered under the grid. Also applies the asset's font size/style/color to the
// panel's text (found automatically among this GameObject's children if not assigned
// — every Dialog Panel has exactly one TMP_Text child, so no per-scene wiring is
// needed). Runs on Start (runtime), OnValidate (Editor, when this component's own
// fields change), and via ApplyIfUsing (called by WordleDialogPanelLayout.OnValidate)
// the instant the shared asset itself is edited, so changes show up immediately in
// whichever scene is open without entering Play mode. Also exposed as a context-menu
// action for a one-click manual re-apply.
[RequireComponent(typeof(RectTransform))]
public class WordleDialogPanelLayoutApplier : MonoBehaviour
{
    [Tooltip("Shared layout asset used by every Wordle puzzle scene's Dialog Panel.")]
    [SerializeField] private WordleDialogPanelLayout layout;

    [Tooltip("This Dialog Panel's RectTransform. Defaults to this GameObject's own RectTransform.")]
    [SerializeField] private RectTransform dialogPanelRect;

    [Tooltip("This Dialog Panel's text. Leave unassigned to auto-find it among this GameObject's children.")]
    [SerializeField] private TMP_Text dialogText;

    private void Reset()
    {
        dialogPanelRect = GetComponent<RectTransform>();
        dialogText = GetComponentInChildren<TMP_Text>();
    }

    private void Start()
    {
        Apply();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Note: this can print a harmless "SendMessage cannot be called during ...
        // OnValidate" console warning (Unity's internal layout rebuild reacting to the
        // anchor/size change) — cosmetic only, the values below are still applied
        // correctly every time. Deferring via EditorApplication.delayCall to avoid the
        // warning was tried and found unreliable in this environment (it did not fire
        // even after several seconds), so applying synchronously here instead.
        Apply();
    }
#endif

    /// <summary>Called by WordleDialogPanelLayout.OnValidate on every applier it can find
    /// whenever the shared asset's own values change; re-applies only if this instance
    /// actually uses that asset.</summary>
    public void ApplyIfUsing(WordleDialogPanelLayout changedLayout)
    {
        if (layout == changedLayout)
        {
            Apply();
        }
    }

    [ContextMenu("Apply Dialog Panel Layout")]
    private void Apply()
    {
        if (layout == null || dialogPanelRect == null)
        {
            return;
        }

        float left = Mathf.Clamp(layout.leftPaddingPercent, 0f, 50f) / 100f;
        float right = Mathf.Clamp(layout.rightPaddingPercent, 0f, 50f) / 100f;

        Vector2 anchorMin = dialogPanelRect.anchorMin;
        Vector2 anchorMax = dialogPanelRect.anchorMax;
        Vector2 sizeDelta = dialogPanelRect.sizeDelta;
        Vector2 anchoredPosition = dialogPanelRect.anchoredPosition;

        anchorMin.x = left;
        anchorMax.x = 1f - right;
        sizeDelta.x = 0f;
        anchoredPosition.x = 0f;

        dialogPanelRect.anchorMin = anchorMin;
        dialogPanelRect.anchorMax = anchorMax;
        dialogPanelRect.sizeDelta = sizeDelta;
        dialogPanelRect.anchoredPosition = anchoredPosition;
        dialogPanelRect.pivot = new Vector2(0.5f, dialogPanelRect.pivot.y);

        TMP_Text text = dialogText != null ? dialogText : GetComponentInChildren<TMP_Text>();
        if (text != null)
        {
            text.fontSize = layout.fontSize;
            text.fontStyle = layout.fontStyle;
            text.color = layout.fontColor;
        }
    }
}
