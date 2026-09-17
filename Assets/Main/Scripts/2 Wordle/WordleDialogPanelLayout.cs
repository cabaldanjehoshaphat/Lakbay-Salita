using TMPro;
using UnityEngine;

// Shared layout + text settings for the Dialog Panel in every Wordle puzzle scene
// under Assets/Main/Scenes/2 Wordle — one asset referenced by the
// WordleDialogPanelLayoutApplier component in each scene. Editing these values
// refreshes every applier in the currently open scene immediately (see OnValidate
// below), the same way WordleTextStyle drives grid-cell font/size live. (The dialog
// text's font asset itself is still chosen on WordleTextStyle — this asset covers
// size/style/color plus this panel's own padding.)
[CreateAssetMenu(fileName = "WordleDialogPanelLayout", menuName = "Lakbay-Salita/Wordle Dialog Panel Layout")]
public class WordleDialogPanelLayout : ScriptableObject
{
    [Header("Padding")]
    [Range(0f, 50f)]
    [Tooltip("Empty space left of the Dialog Panel, as a percentage of the screen width.")]
    public float leftPaddingPercent = 30f;

    [Range(0f, 50f)]
    [Tooltip("Empty space right of the Dialog Panel, as a percentage of the screen width.")]
    public float rightPaddingPercent = 30f;

    [Header("Text")]
    public float fontSize = 36f;
    public FontStyles fontStyle = FontStyles.Normal;
    public Color fontColor = Color.black;

#if UNITY_EDITOR
    /// <summary>Pushes a live update to every WordleDialogPanelLayoutApplier in the
    /// currently open scene that uses this asset, the instant a value changes in the
    /// Inspector.</summary>
    private void OnValidate()
    {
        var appliers = Object.FindObjectsOfType<WordleDialogPanelLayoutApplier>();
        foreach (var applier in appliers)
        {
            applier.ApplyIfUsing(this);
        }
    }
#endif
}
