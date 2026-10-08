using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// CrosswordCategoryBootstrap
/// Sets up the Crossword category play scene ("Crossword - Play") before the game starts. That one scene plays every category puzzle: it reads
/// the chosen language, category and size from CrosswordSession, hands the puzzle to the CrosswordInputController (SetPuzzleOverride, which
/// then builds the grid and the clues instead of loading one of the old numbered levels), switches the CrosswordGameController into category
/// mode and fills the category chip in the top bar (colour dot, category name and "Medium · 8 words"). It runs very early (Awake, execution
/// order -200). If the scene is opened directly without a session it plays the Small puzzle of the first category. References are assigned in
/// the scene by CrosswordCategoryBuilder.
/// </summary>
[DefaultExecutionOrder(-200)]
public class CrosswordCategoryBootstrap : MonoBehaviour
{
    [SerializeField] private CrosswordInputController input;
    [SerializeField] private CrosswordGameController controller;

    [Header("Category chip (top bar)")]
    [SerializeField] private Image categoryDot;
    [SerializeField] private TMP_Text categoryNameLabel;
    [SerializeField] private TMP_Text categorySizeLabel;

    private void Awake()
    {
        int language = Mathf.Clamp(CrosswordSession.Language, 0, 2);
        int category = CrosswordSession.Category;
        int size = CrosswordSession.Size;

        CrosswordPuzzleDef def = CrosswordCategoryData.Puzzle(language, category, size);
        if (def == null)
        {
            category = 0;
            size = 0;
            CrosswordSession.Category = 0;
            CrosswordSession.Size = 0;
            def = CrosswordCategoryData.Puzzle(language, 0, 0);
        }
        if (def == null)
        {
            Debug.LogError("CrosswordCategoryBootstrap: no Crossword puzzle data (Data/Crossword/Puzzles, see Resources/LevelDataRegistry.asset).");
            return;
        }

        CrosswordPuzzle puzzle = CrosswordCategoryData.ToPuzzle(language, size, def);
        CrosswordCategoryDef categoryDef = CrosswordCategoryData.Category(language, category);
        string sourceName = (categoryDef != null ? categoryDef.name : "Category") + " · " + def.size;

        if (input != null) input.SetPuzzleOverride(puzzle, sourceName);
        if (controller != null) controller.ConfigureCategory(language, category, size);

        if (categoryDot != null) categoryDot.color = WordleData.CategoryColor(category);
        if (categoryNameLabel != null && categoryDef != null) categoryNameLabel.text = categoryDef.name;
        if (categorySizeLabel != null) categorySizeLabel.text = def.size + " · " + def.words.Count + " words";
    }
}
