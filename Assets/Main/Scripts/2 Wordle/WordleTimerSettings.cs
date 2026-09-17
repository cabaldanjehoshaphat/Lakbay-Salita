using TMPro;
using UnityEngine;

// Shared timer settings (text style, delay, position) for every Wordle puzzle scene
// under Assets/Main/Scenes/2 Wordle — one asset referenced by the WordleTimer
// component in each scene. Editing these values refreshes every WordleTimer in the
// currently open scene immediately (see OnValidate below), the same way
// WordleTextStyle and WordleDialogPanelLayout drive their own scenes live.
//
// The starting duration (minutes/seconds) is NOT here — puzzles vary in difficulty
// (3x3 up to 7x7 grids), so each scene sets its own duration directly on that scene's
// WordleTimer component instead of sharing one value from this asset.
[CreateAssetMenu(fileName = "WordleTimerSettings", menuName = "Lakbay-Salita/Wordle Timer Settings")]
public class WordleTimerSettings : ScriptableObject
{
    [Header("Delay Before Timer Starts")]
    [Tooltip("Seconds to count down before the real timer starts running. 0 = no delay, the real timer starts immediately (previous behaviour).")]
    [Range(0f, 30f)]
    public float delaySeconds = 0f;

    [Header("Text Style")]
    public TMP_FontAsset font;
    public float fontSize = 36f;
    public FontStyles fontStyle = FontStyles.Normal;

    [Header("Position")]
    [Tooltip("Anchored X position of the Timer widget, relative to its current anchor (top-center). Negative moves it left, positive moves it right, 0 keeps it centered.")]
    public float horizontalPosition = 0f;

#if UNITY_EDITOR
    /// <summary>Pushes a live update to every WordleTimer in the currently open scene
    /// that uses this asset, the instant a value changes in the Inspector.</summary>
    private void OnValidate()
    {
        var timers = Object.FindObjectsOfType<WordleTimer>();
        foreach (var timer in timers)
        {
            timer.ApplyIfUsing(this);
        }
    }
#endif
}
