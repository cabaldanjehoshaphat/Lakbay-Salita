using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// MainMenuMotion
/// Brings the Main Menu to life while the game is running (everything stays still in the editor so the scene is easy to edit):
///  - Floating letter tiles bob, sway and rock gently, each at its own pace.
///  - The LAKBAY title tiles start gray and scrambled, then flip one after another into the right order and colour.
///  - The three wave layers (and their crest lines) scroll sideways at different speeds.
///  - The boat bobs and tilts, and the clouds drift slowly.
/// Assign the lists on the "MenuMotion" object; every animation can be tuned or switched off with the fields below.
/// Uses unscaled time, so it keeps moving even if Time.timeScale is 0.
/// </summary>
public class MainMenuMotion : MonoBehaviour
{
    [Header("Floating tiles")]
    [SerializeField] private RectTransform[] floatingTiles;
    [SerializeField] private float bobPixels = 14f;
    [SerializeField] private float swayPixels = 8f;
    [SerializeField] private float tiltDegrees = 6f;

    [Header("Title tiles (in spelling order)")]
    [SerializeField] private RectTransform[] titleTiles;
    [SerializeField] private Image[] titleTileImages;
    [SerializeField] private bool playTitleIntro = true;
    [SerializeField] private float introDelay = 0.5f;
    [SerializeField] private float introStagger = 0.16f;
    [SerializeField] private float introFlipSeconds = 0.55f;
    [SerializeField] private Color scrambledColor = new Color32(138, 138, 128, 255);

    [Header("Waves (fill and crest of a layer share one speed)")]
    [SerializeField] private RawImage[] waveImages;
    [SerializeField] private float[] waveSpeeds;

    [Header("Boat and clouds")]
    [SerializeField] private RectTransform boat;
    [SerializeField] private float boatBobPixels = 8f;
    [SerializeField] private float boatTiltDegrees = 3f;
    [SerializeField] private RectTransform[] clouds;
    [SerializeField] private float cloudDriftPixels = 40f;

    private Vector2[] _tileBasePos;
    private float[] _tileBaseRot;
    private Vector2 _boatBasePos;
    private Vector2[] _cloudBasePos;

    private void Awake()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        _tileBasePos = new Vector2[floatingTiles.Length];
        _tileBaseRot = new float[floatingTiles.Length];
        for (int i = 0; i < floatingTiles.Length; i++)
        {
            _tileBasePos[i] = floatingTiles[i].anchoredPosition;
            _tileBaseRot[i] = floatingTiles[i].localEulerAngles.z;
        }

        if (boat != null) _boatBasePos = boat.anchoredPosition;

        _cloudBasePos = new Vector2[clouds.Length];
        for (int i = 0; i < clouds.Length; i++)
        {
            _cloudBasePos[i] = clouds[i].anchoredPosition;
        }

        if (playTitleIntro && titleTiles != null && titleTiles.Length > 1)
        {
            StartCoroutine(TitleIntro());
        }
    }

    private void Update()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        float t = Time.unscaledTime;

        for (int i = 0; i < floatingTiles.Length; i++)
        {
            float speed = 0.6f + (i % 5) * 0.13f;
            float phase = i * 1.7f;
            Vector2 offset = new Vector2(Mathf.Sin(t * speed * 0.8f + phase) * swayPixels, Mathf.Sin(t * speed + phase * 1.3f) * bobPixels);
            floatingTiles[i].anchoredPosition = _tileBasePos[i] + offset;
            floatingTiles[i].localRotation = Quaternion.Euler(0f, 0f, _tileBaseRot[i] + Mathf.Sin(t * speed * 0.7f + phase) * tiltDegrees);
        }

        if (waveImages != null && waveSpeeds != null)
        {
            for (int i = 0; i < waveImages.Length && i < waveSpeeds.Length; i++)
            {
                if (waveImages[i] == null) continue;
                Rect r = waveImages[i].uvRect;
                r.x = Mathf.Repeat(r.x + waveSpeeds[i] * Time.unscaledDeltaTime, 1f);
                waveImages[i].uvRect = r;
            }
        }

        if (boat != null)
        {
            boat.anchoredPosition = _boatBasePos + new Vector2(0f, Mathf.Sin(t * 1.1f) * boatBobPixels);
            boat.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(t * 0.9f + 0.6f) * boatTiltDegrees);
        }

        for (int i = 0; i < clouds.Length; i++)
        {
            clouds[i].anchoredPosition = _cloudBasePos[i] + new Vector2(Mathf.Sin(t * (0.07f + i * 0.02f) + i) * cloudDriftPixels, 0f);
        }
    }

    private IEnumerator TitleIntro()
    {
        int n = titleTiles.Length;
        var finalPos = new Vector2[n];
        var finalColor = new Color[n];
        for (int i = 0; i < n; i++)
        {
            finalPos[i] = titleTiles[i].anchoredPosition;
            finalColor[i] = titleTileImages[i].color;
        }

        // Scrambled start: every tile sits in another tile's slot, in gray.
        int[] scramble = { 2, 5, 4, 0, 1, 3 };
        for (int i = 0; i < n; i++)
        {
            titleTiles[i].anchoredPosition = finalPos[scramble[i] % n];
            titleTileImages[i].color = scrambledColor;
        }

        yield return new WaitForSecondsRealtime(introDelay);

        float total = introStagger * (n - 1) + introFlipSeconds;
        float elapsed = 0f;
        Vector2[] startPos = new Vector2[n];
        for (int i = 0; i < n; i++) startPos[i] = titleTiles[i].anchoredPosition;

        while (elapsed < total)
        {
            elapsed += Time.unscaledDeltaTime;
            for (int i = 0; i < n; i++)
            {
                float u = Mathf.Clamp01((elapsed - introStagger * i) / introFlipSeconds);
                float eased = Mathf.SmoothStep(0f, 1f, u);
                titleTiles[i].anchoredPosition = Vector2.Lerp(startPos[i], finalPos[i], eased);
                titleTiles[i].localScale = new Vector3(Mathf.Max(0.02f, Mathf.Abs(Mathf.Cos(u * Mathf.PI))), 1f, 1f);
                titleTileImages[i].color = u >= 0.5f ? finalColor[i] : scrambledColor;
            }
            yield return null;
        }

        for (int i = 0; i < n; i++)
        {
            titleTiles[i].anchoredPosition = finalPos[i];
            titleTiles[i].localScale = Vector3.one;
            titleTileImages[i].color = finalColor[i];
        }
    }
}
