using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// WordleRowsColumnGenerator
/// Generates a rows x columns grid of letter tiles, configured entirely from the
/// Inspector: rows, columns, and per-cell pixel spacing are plain serialized fields
/// (edited directly on the component, not typed into runtime UI), and the grid is built
/// via the "Generate" button in this component's custom Inspector
/// (WordleRowsColumnGeneratorEditor) — the same pattern this project already uses for
/// BoardDataDrawer's "Fill Up With Random Letters" button. Cells are plain display
/// tiles (Image + TMP_Text, no TMP_InputField) — matching how
/// Assets/Sub/Wordle_Imported/Scripts/Tile.cs already does it in this project — since
/// letters are driven entirely by keyboard input (see WordleKeyboardTyper) rather than
/// clicking into a cell to type. All references (cell prefab, grid parent) are plain
/// serialized fields with no static/singleton state and no GameObject.Find lookups, so
/// this component keeps working correctly whether the whole scene is duplicated or its
/// root GameObject is saved and instantiated as a prefab elsewhere.
/// </summary>
public class WordleRowsColumnGenerator : MonoBehaviour
{
    [Header("Grid Size")]
    [Tooltip("How many rows to generate.")]
    public int rows = 3;

    [Tooltip("How many columns to generate.")]
    public int columns = 5;

    [Tooltip("Distance between each cell, in pixels.")]
    public float spacing = 10f;

    [Header("Cell Setup")]
    [Tooltip("Prefab for one letter cell. Should have an Image and a TMP_Text.")]
    public GameObject cellPrefab;

    [Tooltip("Parent RectTransform the generated cells are placed under.")]
    public RectTransform gridParent;

    [Tooltip("Width/height of each cell.")]
    public Vector2 cellSize = new Vector2(80f, 80f);

    [Header("Behaviour")]
    [Tooltip("If true, the grid is (re)generated automatically when the scene starts playing — so cells and their Cells list are always fresh for WordleKeyboardTyper, even if you only generated in the Editor beforehand.")]
    public bool generateOnStart = true;

    private void Start()
    {
        if (generateOnStart)
        {
            Generate();
        }
    }


    /// <summary>The generated cells' text components, in row-major order (row 0 left-to-right,
    /// then row 1, etc). WordleKeyboardTyper types into these in sequence.</summary>
    public List<TMP_Text> Cells { get; private set; } = new List<TMP_Text>();

    /// <summary>Clears any previously generated cells and lays out a fresh rows x columns grid
    /// using this component's rows/columns/spacing fields. Called from the "Generate" button
    /// in the custom Inspector (or via this ContextMenu entry).</summary>
    [ContextMenu("Generate")]
    public void Generate()
    {
        if (gridParent == null || cellPrefab == null)
        {
            Debug.LogWarning("WordleRowsColumnGenerator: missing gridParent or cellPrefab.");
            return;
        }

        int safeRows = Mathf.Max(1, rows);
        int safeColumns = Mathf.Max(1, columns);
        float safeSpacing = Mathf.Max(0f, spacing);

        Clear();
        Cells = new List<TMP_Text>(safeRows * safeColumns);

        float startX = -(safeColumns - 1) * (cellSize.x + safeSpacing) * 0.5f;
        float startY = (safeRows - 1) * (cellSize.y + safeSpacing) * 0.5f;

        for (int row = 0; row < safeRows; row++)
        {
            for (int col = 0; col < safeColumns; col++)
            {
                GameObject cell = Instantiate(cellPrefab, gridParent);
                cell.name = $"Cell_{row}_{col}";

                RectTransform cellRect = cell.GetComponent<RectTransform>();
                if (cellRect != null)
                {
                    cellRect.sizeDelta = cellSize;
                    cellRect.anchoredPosition = new Vector2(
                        startX + col * (cellSize.x + safeSpacing),
                        startY - row * (cellSize.y + safeSpacing));
                }

                TMP_Text letterText = cell.GetComponentInChildren<TMP_Text>();
                if (letterText != null)
                {
                    letterText.text = string.Empty;
                    Cells.Add(letterText);
                }
            }
        }
    }

    /// <summary>Removes all previously generated child cells from the grid parent.</summary>
    private void Clear()
    {
        for (int i = gridParent.childCount - 1; i >= 0; i--)
        {
            Transform child = gridParent.GetChild(i);
            if (Application.isPlaying)
            {
                Destroy(child.gameObject);
            }
            else
            {
                DestroyImmediate(child.gameObject);
            }
        }
        Cells.Clear();
    }
}
