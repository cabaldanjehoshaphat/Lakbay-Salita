using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// WordleTileView
/// Look and small animations of one letter tile in the redesigned Wordle grid.
/// A tile is a rounded border Image with a rounded fill Image inside it (the fill is the Image whose colour
/// WordleWordVerifier sets) and the letter text inside the fill. It has four looks:
///  - empty: translucent white with a white border,
///  - typed: cream fill with a dark border and dark letter (with a small pop when the letter appears),
///  - cursor: the empty tile that receives the next letter gets an amber border,
///  - result: after Enter the tile flips and takes the green / yellow / gray colour with a white letter.
/// SetInvalid() flashes the border red (a word that is not in the dictionary).
/// Used by WordleKeyboardTyper and WordleWordVerifier; put on the root of the WordleTile prefab.
/// </summary>
public class WordleTileView : MonoBehaviour
{
    [SerializeField] private Image border;
    [SerializeField] private Image fill;
    [SerializeField] private TMP_Text letter;

    [Header("Colours")]
    [SerializeField] private Color emptyBorder = new Color(1f, 1f, 1f, 0.9f);
    [SerializeField] private Color emptyFill = new Color(1f, 1f, 1f, 0.38f);
    [SerializeField] private Color typedBorder = new Color32(59, 42, 26, 255);
    [SerializeField] private Color typedFill = new Color32(253, 248, 238, 255);
    [SerializeField] private Color cursorBorder = new Color32(245, 166, 35, 255);
    [SerializeField] private Color cursorFill = new Color32(255, 247, 228, 255);
    [SerializeField] private Color invalidBorder = new Color32(192, 57, 43, 255);
    [SerializeField] private Color inkLetter = new Color32(59, 42, 26, 255);
    [SerializeField] private Color resultLetter = Color.white;

    private bool hasResult;
    private bool hasLetter;
    private bool isCursor;
    private bool invalid;
    private Coroutine running;

    /// <summary>True once the tile has taken its green / yellow / gray colour.</summary>
    public bool HasResult
    {
        get { return hasResult; }
    }

    /// <summary>A letter was typed into this tile.</summary>
    public void SetTyped()
    {
        if (hasResult)
        {
            return;
        }
        hasLetter = true;
        invalid = false;
        Refresh();
        Pop();
    }

    /// <summary>The tile was emptied again (Backspace).</summary>
    public void SetEmpty()
    {
        if (hasResult)
        {
            return;
        }
        hasLetter = false;
        invalid = false;
        Refresh();
    }

    /// <summary>Marks this tile as the one that receives the next letter.</summary>
    public void SetCursor(bool value)
    {
        if (hasResult || isCursor == value)
        {
            return;
        }
        isCursor = value;
        Refresh();
    }

    /// <summary>Shows or clears the red "not a word" border.</summary>
    public void SetInvalid(bool value)
    {
        if (hasResult || invalid == value)
        {
            return;
        }
        invalid = value;
        Refresh();
    }

    /// <summary>Flips the tile (after delay seconds) and gives it its result colour.</summary>
    public void SetResult(Color color, float delay, float flipSeconds)
    {
        hasResult = true;
        if (running != null)
        {
            StopCoroutine(running);
        }
        if (!gameObject.activeInHierarchy)
        {
            ApplyResult(color);
            return;
        }
        running = StartCoroutine(Flip(color, delay, flipSeconds));
    }

    /// <summary>Colours the tile straight away, without the flip (result card tiles).</summary>
    public void ShowResultNow(Color color, string text)
    {
        hasResult = true;
        if (letter != null)
        {
            letter.text = text;
        }
        ApplyResult(color);
    }

    private void ApplyResult(Color color)
    {
        if (border != null) border.color = color;
        if (fill != null) fill.color = color;
        if (letter != null) letter.color = resultLetter;
    }

    private void Refresh()
    {
        if (border == null || fill == null)
        {
            return;
        }
        if (invalid)
        {
            border.color = invalidBorder;
            fill.color = hasLetter ? typedFill : emptyFill;
        }
        else if (hasLetter)
        {
            border.color = typedBorder;
            fill.color = typedFill;
        }
        else if (isCursor)
        {
            border.color = cursorBorder;
            fill.color = cursorFill;
        }
        else
        {
            border.color = emptyBorder;
            fill.color = emptyFill;
        }
        if (letter != null)
        {
            letter.color = inkLetter;
        }
    }

    private void Pop()
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }
        if (running != null)
        {
            StopCoroutine(running);
        }
        running = StartCoroutine(PopRoutine());
    }

    private IEnumerator PopRoutine()
    {
        float t = 0f;
        while (t < 0.14f)
        {
            t += Time.unscaledDeltaTime;
            float s = 1f + 0.14f * Mathf.Sin(Mathf.Clamp01(t / 0.14f) * Mathf.PI);
            transform.localScale = new Vector3(s, s, 1f);
            yield return null;
        }
        transform.localScale = Vector3.one;
        running = null;
    }

    private IEnumerator Flip(Color color, float delay, float flipSeconds)
    {
        float wait = 0f;
        while (wait < delay)
        {
            wait += Time.unscaledDeltaTime;
            yield return null;
        }

        float duration = Mathf.Max(0.05f, flipSeconds);
        float t = 0f;
        bool swapped = false;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float u = Mathf.Clamp01(t / duration);
            if (u >= 0.5f && !swapped)
            {
                swapped = true;
                ApplyResult(color);
            }
            transform.localScale = new Vector3(1f, Mathf.Max(0.02f, Mathf.Abs(Mathf.Cos(u * Mathf.PI))), 1f);
            yield return null;
        }
        ApplyResult(color);
        transform.localScale = Vector3.one;
        running = null;
    }
}
