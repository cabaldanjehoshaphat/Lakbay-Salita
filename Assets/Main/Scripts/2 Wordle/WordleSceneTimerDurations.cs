using System.Collections.Generic;
using UnityEngine;

// One entry in WordleSceneTimerDurations: the starting duration for a single Wordle
// puzzle scene, matched by scene name.
[System.Serializable]
public class WordleSceneTimerEntry
{
    [Tooltip("Exact scene name (no .unity extension), e.g. \"puzzle-wordle-generator 1 Cebuano\".")]
    public string sceneName;

    [Range(0, 59)]
    public int minutes = 5;

    [Range(0, 59)]
    public int seconds = 0;
}

// Central list of starting timer durations for every Wordle puzzle scene under
// Assets/Main/Scenes/2 Wordle (Cebuano/Ilonggo/Tagalog) — one asset, one entry per
// scene, so every scene's duration can be set from a single Inspector view instead of
// opening each scene individually. Each scene's WordleTimer looks up its own entry by
// matching its scene's name at Start/OnValidate.
[CreateAssetMenu(fileName = "WordleSceneTimerDurations", menuName = "Lakbay-Salita/Wordle Scene Timer Durations")]
public class WordleSceneTimerDurations : ScriptableObject
{
    public List<WordleSceneTimerEntry> scenes = new List<WordleSceneTimerEntry>();

    /// <summary>Looks up the duration for the given scene name. Returns false (and
    /// leaves minutes/seconds at 0) if no matching entry exists.</summary>
    public bool TryGetDuration(string sceneName, out int minutes, out int seconds)
    {
        foreach (var entry in scenes)
        {
            if (entry.sceneName == sceneName)
            {
                minutes = entry.minutes;
                seconds = entry.seconds;
                return true;
            }
        }

        minutes = 0;
        seconds = 0;
        return false;
    }

#if UNITY_EDITOR
    /// <summary>Pushes a live update to every WordleTimer in the currently open scene
    /// that uses this asset, the instant a value changes in the Inspector.</summary>
    private void OnValidate()
    {
        var timers = Object.FindObjectsOfType<WordleTimer>();
        foreach (var timer in timers)
        {
            timer.ApplyIfUsingDurations(this);
        }
    }
#endif
}
