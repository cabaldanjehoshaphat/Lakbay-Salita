using TMPro;
using UnityEngine;

// Applies a shared WordleTextStyle asset's font/size to this scene's grid cells, and
// just the font asset (not size/style/color — those live on WordleDialogPanelLayout
// instead) to the Dialog Panel text. Runs on Start (runtime), OnValidate (Editor, when
// this component's own fields change), and — via ApplyIfUsing, called by
// WordleTextStyle.OnValidate — the instant the shared style asset itself is edited
// in the Inspector, so changes show up immediately in whichever scene is open
// without entering Play mode. Also exposed as a context-menu action for a
// one-click manual re-apply.
//
// Cell text components are looked up live from the grid parent's actual children
// (GetComponentsInChildren) rather than WordleRowsColumnGenerator.Cells, since that
// list is a plain runtime field that's empty until Generate() runs in the current
// Editor session — the grid's GameObjects themselves persist in the scene either way.
public class WordleTextStyleApplier : MonoBehaviour
{
    [Tooltip("Shared style asset used by every Wordle puzzle scene.")]
    [SerializeField] private WordleTextStyle style;

    [Tooltip("This scene's grid generator, whose cells get styled.")]
    [SerializeField] private WordleRowsColumnGenerator generator;

    [Tooltip("This scene's Dialog Panel text.")]
    [SerializeField] private TMP_Text dialogText;

    private void Start()
    {
        Apply();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Note: this can print a harmless "SendMessage cannot be called during ...
        // OnValidate" console warning (TMP's internal layout rebuild reacting to the
        // font/size change) — cosmetic only, the values below are still applied
        // correctly every time. Deferring via EditorApplication.delayCall to avoid the
        // warning was tried and found unreliable in this environment (it did not fire
        // even after several seconds), so applying synchronously here instead.
        Apply();
    }
#endif

    /// <summary>Called by WordleTextStyle.OnValidate on every WordleTextStyleApplier it can
    /// find whenever the shared asset's own values change; re-applies only if this
    /// instance actually uses that asset.</summary>
    public void ApplyIfUsing(WordleTextStyle changedStyle)
    {
        if (style == changedStyle)
        {
            Apply();
        }
    }

    [ContextMenu("Apply Text Style")]
    private void Apply()
    {
        if (style == null)
        {
            return;
        }

        if (generator != null && generator.gridParent != null)
        {
            var cells = generator.gridParent.GetComponentsInChildren<TMP_Text>(true);
            foreach (TMP_Text cell in cells)
            {
                if (style.cellFont != null)
                {
                    cell.font = style.cellFont;
                }

                cell.fontSize = style.cellFontSize;
                cell.fontStyle = style.cellFontStyle;
            }
        }

        if (dialogText != null && style.dialogFont != null)
        {
            dialogText.font = style.dialogFont;
        }
    }
}
