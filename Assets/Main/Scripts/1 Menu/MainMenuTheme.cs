using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Part of the day used to pick the Main Menu's sky and sea.</summary>
public enum DayPeriod
{
    Morning,
    Noon,
    Afternoon,
    Night
}

/// <summary>Inspector choice: follow the device clock, or force one period (handy for previewing in the editor).</summary>
public enum ThemePreview
{
    Auto,
    Morning,
    Noon,
    Afternoon,
    Night
}

/// <summary>Colours and decoration strengths of one time-of-day look.</summary>
[Serializable]
public class SkyTheme
{
    public Color skyTop = Color.white;
    public Color skyBottom = Color.white;
    public Color[] waveFill = new Color[3];
    public Color[] waveCrest = new Color[3];
    [Range(0f, 1f)] public float clouds;
    [Range(0f, 1f)] public float waveMarks;
    [Range(0f, 1f)] public float ripples;
    [Range(0f, 1f)] public float gulls;
    [Range(0f, 1f)] public float boat;
    [Range(0f, 1f)] public float sun;
    [Range(0f, 1f)] public float night;

    public static SkyTheme Lerp(SkyTheme a, SkyTheme b, float t)
    {
        var r = new SkyTheme();
        r.skyTop = Color.Lerp(a.skyTop, b.skyTop, t);
        r.skyBottom = Color.Lerp(a.skyBottom, b.skyBottom, t);
        for (int i = 0; i < 3; i++)
        {
            r.waveFill[i] = Color.Lerp(a.waveFill[i], b.waveFill[i], t);
            r.waveCrest[i] = Color.Lerp(a.waveCrest[i], b.waveCrest[i], t);
        }
        r.clouds = Mathf.Lerp(a.clouds, b.clouds, t);
        r.waveMarks = Mathf.Lerp(a.waveMarks, b.waveMarks, t);
        r.ripples = Mathf.Lerp(a.ripples, b.ripples, t);
        r.gulls = Mathf.Lerp(a.gulls, b.gulls, t);
        r.boat = Mathf.Lerp(a.boat, b.boat, t);
        r.sun = Mathf.Lerp(a.sun, b.sun, t);
        r.night = Mathf.Lerp(a.night, b.night, t);
        return r;
    }
}

/// <summary>
/// MainMenuTheme
/// Gives the Main Menu a different sky and sea for each part of the day, chosen from the device clock:
/// Morning 05:00-10:59, Noon 11:00-15:59, Afternoon 16:00-18:59, Night 19:00-04:59 (hours are editable below).
/// It recolours the sky (a solid base plus a gradient overlay), the three wave layers and their crest lines, and fades
/// decoration groups (clouds, wave marks, ripples, gulls, boat, sun, moon and stars) in or out. While the menu stays open
/// it re-checks the clock and fades smoothly to the next period.
///
/// How to use: it lives on the "MenuTheme" object of the Main Menu scene with all references assigned. Use the
/// "Preview" dropdown to look at any period in the editor without waiting for that hour (Auto = follow the clock; the
/// editor shows Noon for Auto). Right-click the component title and choose "Load default themes" to restore the colours.
/// For presentations, the demo keys 1 (Morning), 2 (Noon), 3 (Afternoon), 4 (Night) and 0 (follow the clock) switch the sky while the
/// game runs, with the normal fade; they can be turned off with "Allow Demo Keys".
/// </summary>
public class MainMenuTheme : MonoBehaviour
{
    [Header("Time of day")]
    [SerializeField] private ThemePreview preview = ThemePreview.Auto;
    [SerializeField, Range(0, 23)] private int morningStartHour = 5;
    [SerializeField, Range(0, 23)] private int noonStartHour = 11;
    [SerializeField, Range(0, 23)] private int afternoonStartHour = 16;
    [SerializeField, Range(0, 23)] private int nightStartHour = 19;
    [SerializeField] private float fadeSeconds = 3f;
    [Tooltip("Demo keys (for presentations): 1 = Morning, 2 = Noon, 3 = Afternoon, 4 = Night, 0 = follow the clock again. Works on every screen that has this sky. Untick to turn the keys off.")]
    [SerializeField] private bool allowDemoKeys = true;
    [SerializeField] private float clockCheckSeconds = 20f;

    [Header("Sky")]
    [SerializeField] private Image skyBase;
    [SerializeField] private Image skyTopOverlay;

    [Header("Sea (three layers, back to front)")]
    [SerializeField] private RawImage[] waveFills = new RawImage[3];
    [SerializeField] private RawImage[] waveCrests = new RawImage[3];

    [Header("Decoration groups")]
    [SerializeField] private CanvasGroup clouds;
    [SerializeField] private CanvasGroup waveMarks;
    [SerializeField] private CanvasGroup ripples;
    [SerializeField] private CanvasGroup gulls;
    [SerializeField] private CanvasGroup boat;
    [SerializeField] private CanvasGroup sun;
    [SerializeField] private CanvasGroup night;

    [Header("Looks (Morning, Noon, Afternoon, Night)")]
    [SerializeField] private SkyTheme[] themes = new SkyTheme[4];

    private int _shownIndex = -1;
    private int _fadeFrom = -1;
    private int _fadeTo = -1;
    private float _fadeT;
    private float _nextCheck;

    private void Start()
    {
        if (!Application.isPlaying)
        {
            return;
        }
        _shownIndex = ResolveIndex();
        ApplyTheme(themes[_shownIndex]);
        _nextCheck = Time.unscaledTime + clockCheckSeconds;
    }

    /// <summary>
    /// Demo helper: forces one look right now with the normal fade (0 = Morning, 1 = Noon, 2 = Afternoon, 3 = Night), or
    /// -1 to go back to following the device clock. Used by the demo keys; also callable from other scripts.
    /// </summary>
    public void SetDemoPeriod(int index)
    {
        preview = index < 0 || index > 3 ? ThemePreview.Auto : (ThemePreview)(index + 1);
        int target = ResolveIndex();
        if (_fadeTo >= 0)
        {
            // finish the fade that is still running, then continue from there
            _shownIndex = _fadeTo;
            _fadeTo = -1;
            ApplyTheme(themes[_shownIndex]);
        }
        if (target != _shownIndex)
        {
            _fadeFrom = _shownIndex;
            _fadeTo = target;
            _fadeT = 0f;
        }
    }

    private void HandleDemoKeys()
    {
        if (!allowDemoKeys)
        {
            return;
        }

        // typing a name or a search must not change the sky
        var system = UnityEngine.EventSystems.EventSystem.current;
        if (system != null && system.currentSelectedGameObject != null
            && system.currentSelectedGameObject.GetComponent<TMPro.TMP_InputField>() != null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) SetDemoPeriod(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) SetDemoPeriod(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) SetDemoPeriod(2);
        else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4)) SetDemoPeriod(3);
        else if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0)) SetDemoPeriod(-1);
    }

    private void Update()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        HandleDemoKeys();

        if (_fadeTo >= 0)
        {
            _fadeT += Time.unscaledDeltaTime / Mathf.Max(0.01f, fadeSeconds);
            if (_fadeT >= 1f)
            {
                ApplyTheme(themes[_fadeTo]);
                _shownIndex = _fadeTo;
                _fadeTo = -1;
            }
            else
            {
                ApplyTheme(SkyTheme.Lerp(themes[_fadeFrom], themes[_fadeTo], Mathf.SmoothStep(0f, 1f, _fadeT)));
            }
            return;
        }

        if (Time.unscaledTime >= _nextCheck)
        {
            _nextCheck = Time.unscaledTime + clockCheckSeconds;
            int target = ResolveIndex();
            if (target != _shownIndex)
            {
                _fadeFrom = _shownIndex;
                _fadeTo = target;
                _fadeT = 0f;
            }
        }
    }

    /// <summary>The period for the given clock time using the configured start hours.</summary>
    public DayPeriod PeriodFor(DateTime now)
    {
        int h = now.Hour;
        if (h >= nightStartHour || h < morningStartHour) return DayPeriod.Night;
        if (h >= afternoonStartHour) return DayPeriod.Afternoon;
        if (h >= noonStartHour) return DayPeriod.Noon;
        return DayPeriod.Morning;
    }

    private int ResolveIndex()
    {
        if (preview == ThemePreview.Auto)
        {
            return (int)PeriodFor(DateTime.Now);
        }
        return (int)preview - 1;
    }

    private void ApplyTheme(SkyTheme t)
    {
        if (t == null || t.waveFill == null || t.waveFill.Length < 3)
        {
            return;
        }
        if (skyBase != null) skyBase.color = t.skyBottom;
        if (skyTopOverlay != null) skyTopOverlay.color = t.skyTop;
        for (int i = 0; i < 3; i++)
        {
            if (waveFills != null && i < waveFills.Length && waveFills[i] != null) waveFills[i].color = t.waveFill[i];
            if (waveCrests != null && i < waveCrests.Length && waveCrests[i] != null) waveCrests[i].color = t.waveCrest[i];
        }
        SetAlpha(clouds, t.clouds);
        SetAlpha(waveMarks, t.waveMarks);
        SetAlpha(ripples, t.ripples);
        SetAlpha(gulls, t.gulls);
        SetAlpha(boat, t.boat);
        SetAlpha(sun, t.sun);
        SetAlpha(night, t.night);
    }

    private static void SetAlpha(CanvasGroup g, float a)
    {
        if (g != null) g.alpha = a;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            return;
        }
        UnityEditor.EditorApplication.delayCall += () =>
        {
            if (this == null || themes == null || themes.Length < 4)
            {
                return;
            }
            int index = preview == ThemePreview.Auto ? (int)DayPeriod.Noon : (int)preview - 1;
            if (AlreadyShowing(themes[index]))
            {
                return;
            }
            ApplyTheme(themes[index]);
            MarkTargetsDirty();
        };
    }

    // True when the sky and sea already have this theme's colours, so re-validating does not dirty the scene.
    private bool AlreadyShowing(SkyTheme t)
    {
        if (t == null || t.waveFill == null || t.waveFill.Length < 3 || skyBase == null || skyTopOverlay == null)
        {
            return false;
        }
        if (skyBase.color != t.skyBottom || skyTopOverlay.color != t.skyTop)
        {
            return false;
        }
        for (int i = 0; i < 3; i++)
        {
            // Screens without waves (the Wordle screen) leave these empty; they never count as a mismatch.
            if (waveFills != null && i < waveFills.Length && waveFills[i] != null && waveFills[i].color != t.waveFill[i])
            {
                return false;
            }
        }
        return clouds == null || Mathf.Approximately(clouds.alpha, t.clouds);
    }

    private void MarkTargetsDirty()
    {
        UnityEditor.EditorUtility.SetDirty(this);
        MarkDirty(skyBase);
        MarkDirty(skyTopOverlay);
        foreach (var w in waveFills) MarkDirty(w);
        foreach (var w in waveCrests) MarkDirty(w);
        MarkDirty(clouds); MarkDirty(waveMarks); MarkDirty(ripples); MarkDirty(gulls);
        MarkDirty(boat); MarkDirty(sun); MarkDirty(night);
    }

    private static void MarkDirty(UnityEngine.Object o)
    {
        if (o != null) UnityEditor.EditorUtility.SetDirty(o);
    }

    /// <summary>Editor helper: applies one period right now (used by the scene setup).</summary>
    public void PreviewPeriod(DayPeriod period)
    {
        ApplyTheme(themes[(int)period]);
        MarkTargetsDirty();
    }

    [ContextMenu("Load default themes")]
    public void LoadDefaultThemes()
    {
        themes = new SkyTheme[4];
        themes[(int)DayPeriod.Morning] = MakeTheme("#CDEBFF", "#6FBCEF",
            new[] { "#8FD0F6", "#58AEE6", "#3C93D6" }, new[] { 150, 170, 215 },
            new[] { "#FFFFFF", "#E6F6FF", "#CFEBFF" },
            1f, 1f, 0f, 0f, 0f, 0f, 0f);
        themes[(int)DayPeriod.Noon] = MakeTheme("#BFE6FF", "#5FB4EC",
            new[] { "#7CC4F0", "#4FA9E4", "#3589CF" }, new[] { 110, 190, 225 },
            new[] { "#FFFFFF", "#E6F6FF", "#CFEBFF" },
            1f, 1f, 1f, 1f, 1f, 0f, 0f);
        themes[(int)DayPeriod.Afternoon] = MakeTheme("#FFE3B0", "#FF9E73",
            new[] { "#F27C6B", "#2F8FB5", "#1E6E96" }, new[] { 150, 200, 235 },
            new[] { "#FFE0C0", "#BFE9F5", "#A9DCEB" },
            0.7f, 0f, 0f, 0f, 0f, 1f, 0f);
        themes[(int)DayPeriod.Night] = MakeTheme("#13285C", "#1F5C9E",
            new[] { "#2A6FB5", "#1F5C9E", "#17457E" }, new[] { 140, 170, 225 },
            new[] { "#8FC8F5", "#7DB9EE", "#6AA7E0" },
            0f, 0f, 0f, 0f, 0f, 0f, 1f);
        MarkTargetsDirty();
    }

    private static SkyTheme MakeTheme(string top, string bottom, string[] fills, int[] fillAlphas, string[] crests,
        float clouds, float marks, float ripples, float gulls, float boat, float sun, float night)
    {
        var t = new SkyTheme();
        t.skyTop = Hex(top, 255);
        t.skyBottom = Hex(bottom, 255);
        for (int i = 0; i < 3; i++)
        {
            t.waveFill[i] = Hex(fills[i], fillAlphas[i]);
            t.waveCrest[i] = Hex(crests[i], 210);
        }
        t.clouds = clouds; t.waveMarks = marks; t.ripples = ripples; t.gulls = gulls;
        t.boat = boat; t.sun = sun; t.night = night;
        return t;
    }

    private static Color Hex(string hex, int alpha)
    {
        Color c;
        ColorUtility.TryParseHtmlString(hex, out c);
        c.a = alpha / 255f;
        return c;
    }
#endif
}
