using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// WordSearchBoardView
/// The 10x10 letter grid of the Word Search play screen. It shows the letters (100 labels made in the scene by the design builder) and
/// turns dragging into a selection: press on a letter, drag, and the selection snaps to the nearest straight line (across, down or
/// diagonal, forwards or backwards) and is drawn as a coral capsule. When the finger or mouse is released, SelectionReleased reports the
/// start and end cell; the game controller checks it and calls MarkFound (the capsule stays in the word's own colour) or
/// ClearSelection. FlashCell pulses a ring on one letter for the Hint button. Coordinates: row 0 is the top row, column 0 the left column.
/// The board RectTransform must use a top-left pivot and have an Image (alpha 0, raycast on) so it receives the pointer.
/// </summary>
public class WordSearchBoardView : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform boardRect;
    [SerializeField] private RectTransform capsuleLayer;
    [SerializeField] private TMP_Text[] cellLabels = new TMP_Text[WordSearchGrid.Size * WordSearchGrid.Size];
    [SerializeField] private float cellSize = 82f;
    [SerializeField] private float capsuleThickness = 64f;
    [SerializeField] private Color selectionColor = new Color(0.85f, 0.38f, 0.23f, 0.62f);
    [SerializeField] private Color[] foundColors =
    {
        new Color(0.91f, 0.73f, 0.14f, 0.62f), new Color(0.36f, 0.63f, 0.16f, 0.55f), new Color(0.22f, 0.54f, 0.87f, 0.5f),
        new Color(0.54f, 0.25f, 0.78f, 0.45f), new Color(0.91f, 0.46f, 0.10f, 0.55f), new Color(0.11f, 0.62f, 0.46f, 0.5f),
        new Color(0.83f, 0.33f, 0.49f, 0.5f), new Color(0.38f, 0.37f, 0.35f, 0.4f)
    };

    /// <summary>Raised when the player lets go: startRow, startCol, endRow, endCol of the snapped selection.</summary>
    public event Action<int, int, int, int> SelectionReleased;

    /// <summary>The colour used for the n-th found word.</summary>
    public Color FoundColor(int index)
    {
        return foundColors[Mathf.Abs(index) % foundColors.Length];
    }

    private readonly List<Image> foundCapsules = new List<Image>();
    private Image selectionCapsule;
    private Image hintRing;
    private bool inputEnabled = true;
    private bool dragging;
    private int startRow, startCol, endRow, endCol;
    private Coroutine flashRoutine;

    private void Awake()
    {
        EnsureLayers();
    }

    private void EnsureLayers()
    {
        if (selectionCapsule == null && capsuleLayer != null)
        {
            selectionCapsule = NewCapsule("selection_capsule");
            selectionCapsule.gameObject.SetActive(false);
            hintRing = NewCapsule("hint_ring");
            hintRing.sprite = LibraryUiKit.Circle;
            hintRing.type = Image.Type.Simple;
            hintRing.rectTransform.sizeDelta = new Vector2(cellSize, cellSize);
            hintRing.color = new Color(0.91f, 0.73f, 0.14f, 0.9f);
            hintRing.gameObject.SetActive(false);
        }
    }

    private Image NewCapsule(string objectName)
    {
        var go = new GameObject(objectName, typeof(RectTransform));
        go.layer = 5;
        var rt = (RectTransform)go.transform;
        rt.SetParent(capsuleLayer, false);
        rt.anchorMin = rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        var img = go.AddComponent<Image>();
        img.sprite = LibraryUiKit.Rounded;
        img.type = Image.Type.Sliced;
        img.pixelsPerUnitMultiplier = 0.8f;
        img.raycastTarget = false;
        return img;
    }

    /// <summary>Shows the grid's letters and clears every capsule.</summary>
    public void Build(WordSearchGrid grid)
    {
        if (Application.isPlaying)
        {
            EnsureLayers();
            ClearAll();
        }
        for (int r = 0; r < WordSearchGrid.Size; r++)
        {
            for (int c = 0; c < WordSearchGrid.Size; c++)
            {
                TMP_Text label = cellLabels[r * WordSearchGrid.Size + c];
                if (label != null)
                {
                    label.text = grid.letters[r, c].ToString();
                }
            }
        }
    }

    public bool InputEnabled
    {
        get { return inputEnabled; }
        set
        {
            inputEnabled = value;
            if (!value)
            {
                CancelSelection();
            }
        }
    }

    /// <summary>Removes every capsule (found words, selection, hint ring).</summary>
    public void ClearAll()
    {
        foreach (Image img in foundCapsules)
        {
            if (img != null) Destroy(img.gameObject);
        }
        foundCapsules.Clear();
        CancelSelection();
        if (hintRing != null) hintRing.gameObject.SetActive(false);
    }

    public void ClearSelection()
    {
        CancelSelection();
    }

    private void CancelSelection()
    {
        dragging = false;
        if (selectionCapsule != null) selectionCapsule.gameObject.SetActive(false);
    }

    /// <summary>Keeps a capsule on the cells of a found word, in that word's colour.</summary>
    public void MarkFound(int row, int col, int dRow, int dCol, int length, Color color)
    {
        Image img = NewCapsule("found_capsule");
        img.color = color;
        Place(img, row, col, row + dRow * (length - 1), col + dCol * (length - 1));
        foundCapsules.Add(img);
        CancelSelection();
    }

    /// <summary>Pulses a ring on one cell for a moment (used by the Hint button).</summary>
    public void FlashCell(int row, int col, float seconds)
    {
        EnsureLayers();
        if (hintRing == null)
        {
            return;
        }
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }
        flashRoutine = StartCoroutine(Flash(row, col, seconds));
    }

    private IEnumerator Flash(int row, int col, float seconds)
    {
        hintRing.gameObject.SetActive(true);
        hintRing.rectTransform.anchoredPosition = CellCenter(row, col);
        float t = 0f;
        while (t < seconds)
        {
            t += Time.unscaledDeltaTime;
            float pulse = 0.5f + 0.5f * Mathf.Sin(t * 9f);
            float s = Mathf.Lerp(0.9f, 1.15f, pulse);
            hintRing.rectTransform.localScale = new Vector3(s, s, 1f);
            Color c = hintRing.color;
            c.a = Mathf.Lerp(0.35f, 0.85f, pulse);
            hintRing.color = c;
            yield return null;
        }
        hintRing.gameObject.SetActive(false);
        flashRoutine = null;
    }

    // ------------------------------------------------------------------ geometry

    private Vector2 CellCenter(int row, int col)
    {
        return new Vector2((col + 0.5f) * cellSize, -(row + 0.5f) * cellSize);
    }

    /// <summary>Draws a capsule from the centre of one cell to the centre of another.</summary>
    private void Place(Image capsule, int r1, int c1, int r2, int c2)
    {
        Vector2 a = CellCenter(r1, c1);
        Vector2 b = CellCenter(r2, c2);
        Vector2 mid = (a + b) * 0.5f;
        float length = Vector2.Distance(a, b) + capsuleThickness;
        float angle = Mathf.Atan2(b.y - a.y, b.x - a.x) * Mathf.Rad2Deg;
        RectTransform rt = capsule.rectTransform;
        rt.anchoredPosition = mid;
        rt.sizeDelta = new Vector2(length, capsuleThickness);
        rt.localRotation = Quaternion.Euler(0f, 0f, angle);
    }

    private bool TryGetCell(PointerEventData eventData, out int row, out int col)
    {
        row = col = 0;
        Camera cam = eventData.pressEventCamera;
        if (cam == null)
        {
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                cam = canvas.worldCamera;
            }
        }
        Vector2 local;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(boardRect, eventData.position, cam, out local))
        {
            return false;
        }
        // the board uses a top-left pivot: x grows right from 0, y grows down as a negative number
        col = Mathf.FloorToInt(local.x / cellSize);
        row = Mathf.FloorToInt(-local.y / cellSize);
        return true;
    }

    // ------------------------------------------------------------------ pointer input

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!inputEnabled)
        {
            return;
        }
        int r, c;
        if (!TryGetCell(eventData, out r, out c) || r < 0 || c < 0 || r >= WordSearchGrid.Size || c >= WordSearchGrid.Size)
        {
            return;
        }
        dragging = true;
        startRow = endRow = r;
        startCol = endCol = c;
        ShowSelection();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!dragging || !inputEnabled)
        {
            return;
        }
        int r, c;
        if (!TryGetCell(eventData, out r, out c))
        {
            return;
        }
        SnapEnd(r, c);
        ShowSelection();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!dragging)
        {
            return;
        }
        dragging = false;
        bool longer = startRow != endRow || startCol != endCol;
        if (!longer)
        {
            CancelSelection();
            return;
        }
        if (SelectionReleased != null)
        {
            SelectionReleased(startRow, startCol, endRow, endCol);
        }
        else
        {
            CancelSelection();
        }
    }

    /// <summary>Snaps the dragged cell to the nearest of the 8 straight directions from the start cell.</summary>
    private void SnapEnd(int row, int col)
    {
        int dr = row - startRow;
        int dc = col - startCol;
        if (dr == 0 && dc == 0)
        {
            endRow = startRow;
            endCol = startCol;
            return;
        }
        float angle = Mathf.Atan2(dr, dc);
        int step = Mathf.RoundToInt(angle / (Mathf.PI / 4f));
        float snapped = step * (Mathf.PI / 4f);
        int sr = Mathf.RoundToInt(Mathf.Sin(snapped));
        int sc = Mathf.RoundToInt(Mathf.Cos(snapped));
        int steps = Mathf.Max(0, Mathf.RoundToInt((dr * sr + dc * sc) / (float)(sr * sr + sc * sc)));
        while (steps > 0)
        {
            int er = startRow + sr * steps;
            int ec = startCol + sc * steps;
            if (er >= 0 && ec >= 0 && er < WordSearchGrid.Size && ec < WordSearchGrid.Size)
            {
                endRow = er;
                endCol = ec;
                return;
            }
            steps--;
        }
        endRow = startRow;
        endCol = startCol;
    }

    private void ShowSelection()
    {
        if (selectionCapsule == null)
        {
            return;
        }
        selectionCapsule.gameObject.SetActive(true);
        selectionCapsule.color = selectionColor;
        Place(selectionCapsule, startRow, startCol, endRow, endCol);
    }

    /// <summary>The letters from the start cell to the end cell (inclusive), for checking a selection against the words.</summary>
    public static string ReadLine(WordSearchGrid grid, int r1, int c1, int r2, int c2)
    {
        int dr = Math.Sign(r2 - r1);
        int dc = Math.Sign(c2 - c1);
        int n = Math.Max(Math.Abs(r2 - r1), Math.Abs(c2 - c1));
        var chars = new char[n + 1];
        for (int i = 0; i <= n; i++)
        {
            chars[i] = grid.letters[r1 + dr * i, c1 + dc * i];
        }
        return new string(chars);
    }
}
