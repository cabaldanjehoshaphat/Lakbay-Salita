using System;
using TMPro;
using UnityEngine;

/// <summary>
/// CrosswordTextStyle
/// The ONE place that controls every text size in the crossword - grid letters, the small
/// clue-number in the corner of each grid cell, and the Across/Down clue-list text and
/// numbers - plus the grid cell border color/opacity. A single ScriptableObject asset:
/// CrosswordGridGenerator and CrosswordCluePanel both reference this same asset, so tuning
/// one number here (e.g. letterFontSize) updates the whole crossword, not two separate
/// assets for two separate scripts. Edit it in the Inspector and everything already on
/// screen updates immediately (via OnValidate) - no need to regenerate or re-enter Play mode.
/// </summary>
[CreateAssetMenu(fileName = "CrosswordTextStyle", menuName = "Crossword/Text Style")]
public class CrosswordTextStyle : ScriptableObject
{
    [Header("Grid - Letter (what the player types)")]
    public float letterFontSize = 44f;
    public FontStyles letterFontStyle = FontStyles.Bold;
    public Color letterColor = new Color(0.1f, 0.1f, 0.1f);

    [Header("Grid - Cell Number (clue number in the corner)")]
    public float cellNumberFontSize = 16f;
    public Color cellNumberColor = new Color(0.25f, 0.25f, 0.25f);

    [Header("Grid - Cell Border")]
    [Tooltip("Border line color (RGB only - use Border Opacity below to control alpha).")]
    public Color borderColor = new Color(0.25f, 0.22f, 0.18f);

    [Tooltip("Border line opacity, 0 = invisible, 1 = fully solid.")]
    [Range(0f, 1f)]
    public float borderOpacity = 0.7f;

    [Tooltip("Border thickness in pixels (how far the fill is inset from the cell's edge).")]
    public float borderThickness = 3f;

    [Header("Clue Panel - Clue Number (e.g. \"1\", \"2\")")]
    public float clueNumberFontSize = 13f;
    public Color clueNumberColor = new Color(0.15f, 0.15f, 0.15f);

    [Header("Clue Panel - Clue Text")]
    public float clueTextFontSize = 13f;
    public Color clueTextColor = new Color(0.15f, 0.15f, 0.15f);

    /// <summary>Raised whenever a field here is edited in the Inspector, so the grid and
    /// clue panel currently on screen can both refresh without needing to be regenerated.</summary>
    public event Action Changed;

    private void OnValidate()
    {
        Changed?.Invoke();
    }
}
