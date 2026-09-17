using TMPro;
using UnityEngine;

// Countdown timer for a Wordle puzzle scene. The starting duration (minutes/seconds)
// comes from a shared WordleSceneTimerDurations asset, looked up by this GameObject's
// own scene name — one asset holds every puzzle scene's duration, editable from a
// single Inspector view instead of opening each scene individually. Text style, delay,
// and horizontal position come from a separate shared WordleTimerSettings asset (so
// those stay consistent across every scene from one place). If settings.delaySeconds >
// 0, the display first counts down that delay (e.g. a "starting in..." lead-in) before
// switching to the real minutes/seconds countdown; with delaySeconds at 0 (the
// default) or no settings assigned, the real timer starts immediately. Always displays
// MM:SS, clamped at 00:00. Editing either shared asset previews the change live in the
// Editor via OnValidate/ApplyIfUsing. Stops counting altogether once the puzzle's
// WordleKeyboardTyper reports the word solved (found automatically via the scene's
// "WordleGenerator" GameObject — every puzzle scene uses that same name, so no
// per-scene wiring is needed), freezing the display at whatever time was left.
public class WordleTimer : MonoBehaviour
{
    [Header("Duration Lookup (one entry per scene)")]
    [SerializeField] private WordleSceneTimerDurations sceneDurations;

    [Header("Shared Style/Delay/Position")]
    [SerializeField] private WordleTimerSettings settings;
    [SerializeField] private TMP_Text timerText;

    private enum Phase
    {
        Delay,
        Counting,
        Expired
    }

    private Phase phase;
    private float remainingSeconds;
    private WordleKeyboardTyper typer;

    /// <summary>True once the real countdown (not the delay) has reached zero.</summary>
    public bool HasExpired => phase == Phase.Expired;

    private void Start()
    {
        GameObject generatorObject = GameObject.Find("WordleGenerator");
        typer = generatorObject != null ? generatorObject.GetComponent<WordleKeyboardTyper>() : null;
        ResetTimer();
    }

    private void Update()
    {
        if (phase == Phase.Expired || (typer != null && typer.Solved))
        {
            // Word guessed correctly — freeze the countdown right where it is.
            return;
        }

        remainingSeconds -= Time.deltaTime;
        if (remainingSeconds <= 0f)
        {
            if (phase == Phase.Delay)
            {
                phase = Phase.Counting;
                remainingSeconds = MainDurationSeconds();
            }
            else
            {
                remainingSeconds = 0f;
                phase = Phase.Expired;
            }
        }

        UpdateDisplay();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        ApplySettings();
        ResetTimer();
    }
#endif

    /// <summary>Called by WordleTimerSettings.OnValidate on every WordleTimer it can find
    /// whenever that shared asset's own values change; re-applies only if this instance
    /// actually uses that asset.</summary>
    public void ApplyIfUsing(WordleTimerSettings changedSettings)
    {
        if (settings == changedSettings)
        {
            ApplySettings();
            ResetTimer();
        }
    }

    /// <summary>Called by WordleSceneTimerDurations.OnValidate on every WordleTimer it
    /// can find whenever that shared asset's own values change; re-applies only if this
    /// instance actually uses that asset.</summary>
    public void ApplyIfUsingDurations(WordleSceneTimerDurations changedDurations)
    {
        if (sceneDurations == changedDurations)
        {
            ResetTimer();
        }
    }

    /// <summary>Restarts the countdown — from settings.delaySeconds if set, otherwise
    /// straight into this scene's own duration (looked up from sceneDurations).</summary>
    public void ResetTimer()
    {
        float delaySeconds = settings != null ? settings.delaySeconds : 0f;
        if (delaySeconds > 0f)
        {
            phase = Phase.Delay;
            remainingSeconds = delaySeconds;
        }
        else
        {
            phase = Phase.Counting;
            remainingSeconds = MainDurationSeconds();
        }

        UpdateDisplay();
    }

    private float MainDurationSeconds()
    {
        if (sceneDurations != null && sceneDurations.TryGetDuration(gameObject.scene.name, out int minutes, out int seconds))
        {
            return minutes * 60 + seconds;
        }

        Debug.LogWarning($"WordleTimer: no duration entry found for scene \"{gameObject.scene.name}\" in {(sceneDurations != null ? sceneDurations.name : "NULL sceneDurations")}. Defaulting to 5:00.");
        return 5 * 60;
    }

    private void ApplySettings()
    {
        if (settings == null)
        {
            return;
        }

        if (timerText != null)
        {
            if (settings.font != null)
            {
                timerText.font = settings.font;
            }

            timerText.fontSize = settings.fontSize;
            timerText.fontStyle = settings.fontStyle;
        }

        var rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            Vector2 pos = rectTransform.anchoredPosition;
            pos.x = settings.horizontalPosition;
            rectTransform.anchoredPosition = pos;
        }
    }

    private void UpdateDisplay()
    {
        if (timerText == null)
        {
            return;
        }

        int totalSeconds = Mathf.CeilToInt(Mathf.Max(0f, remainingSeconds));
        int displayMinutes = totalSeconds / 60;
        int displaySeconds = totalSeconds % 60;
        timerText.text = $"{displayMinutes:00}:{displaySeconds:00}";
    }
}
