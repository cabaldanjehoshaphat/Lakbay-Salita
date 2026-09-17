using TMPro;
using UnityEngine;

// Shared text-style settings (font + size) for every Wordle puzzle scene under
// Assets/Main/Scenes/2 Wordle — one asset referenced by the WordleTextStyleApplier
// component in each scene. Editing these values here refreshes every
// WordleTextStyleApplier in the currently open scene immediately (see OnValidate
// below), so there's no need to re-apply by hand or enter Play mode.
//
// The Dialog Panel's font SIZE/STYLE/COLOR live on WordleDialogPanelLayout instead
// (alongside that panel's padding) — only its font ASSET is chosen here, paired with
// cellFont as the two font-family choices for this scene.
[CreateAssetMenu(fileName = "WordleTextStyle", menuName = "Lakbay-Salita/Wordle Text Style")]
public class WordleTextStyle : ScriptableObject
{
    [Header("Grid Cells (Rows & Columns)")]
    public TMP_FontAsset cellFont;
    public float cellFontSize = 36f;
    public FontStyles cellFontStyle = FontStyles.Normal;

    [Header("Dialog Panel")]
    public TMP_FontAsset dialogFont;

#if UNITY_EDITOR
    /// <summary>Pushes a live update to every WordleTextStyleApplier in the currently open
    /// scene(s) that uses this asset, the instant a value changes in the Inspector — only
    /// the scene actually open in the Editor can be affected at once, since Unity only
    /// renders one at a time, but every scene picks this up as soon as it's opened.</summary>
    private void OnValidate()
    {
        var appliers = Object.FindObjectsOfType<WordleTextStyleApplier>();
        foreach (var applier in appliers)
        {
            applier.ApplyIfUsing(this);
        }
    }
#endif
}
