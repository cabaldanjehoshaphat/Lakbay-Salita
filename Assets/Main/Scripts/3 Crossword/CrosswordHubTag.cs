using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// CrosswordHubTag
/// The small tag on the Crossword card of the Language Selection hub. It shows how many Crossword puzzles the player has solved in the
/// language that is selected on the hub, with the language name in front, for example "Tagalog · 4 of 21" (7 categories x 3 sizes), and
/// updates as soon as another language is picked. The tag turns gold once at least one puzzle is solved. Put it on the card's tag (the pill with the Image) and assign the language
/// selector and the text label.
/// </summary>
public class CrosswordHubTag : MonoBehaviour
{
    [SerializeField] private LanguageSelector languageSelector;
    [SerializeField] private TMP_Text label;
    [SerializeField] private Image background;
    [SerializeField] private Color solvedColor = new Color32(232, 185, 35, 255);

    private Color normalColor;
    private bool hasNormalColor;
    private int shownLanguage = -1;

    private void OnEnable()
    {
        if (background != null && !hasNormalColor)
        {
            normalColor = background.color;
            hasNormalColor = true;
        }
        shownLanguage = -1;
        Refresh();
    }

    private void Update()
    {
        if (languageSelector != null && (int)languageSelector.SelectedLanguage != shownLanguage)
        {
            Refresh();
        }
    }

    /// <summary>Reads the solved puzzles of the selected language and rewrites the tag.</summary>
    public void Refresh()
    {
        int language = languageSelector != null ? (int)languageSelector.SelectedLanguage : 0;
        shownLanguage = language;

        int total = CrosswordCategoryData.TotalPuzzles(language);
        int solved = CrosswordProgressStore.SolvedTotal(language);

        if (label != null)
        {
            label.text = CrosswordCategoryData.LanguageNames[Mathf.Clamp(language, 0, 2)] + " · " + solved + " of " + total;
        }
        if (background != null && hasNormalColor)
        {
            background.color = solved > 0 ? solvedColor : normalColor;
        }
    }
}
