using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// WordSearchHubTag
/// The small tag on the Word Search card of the Language Selection hub (where Wordle and Crossword show "10 levels"). It shows how many
/// Word Search puzzles the player has solved in the language that is selected on the hub, with the language name in front, for example "Cebuano · 4 of 21", and updates
/// as soon as another language is picked. The tag turns gold once at least one puzzle is solved. Put it on the card's tag
/// (the pill with the Image) and assign the language selector and the text label.
/// </summary>
public class WordSearchHubTag : MonoBehaviour
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

        int total = 0;
        int solved = 0;
        int categories = WordSearchData.CategoryCount(language);
        for (int c = 0; c < categories; c++)
        {
            WordSearchCategory category = WordSearchData.Category(language, c);
            int puzzles = category != null ? category.puzzles.Count : 0;
            total += puzzles;
            solved += WordSearchProgressStore.SolvedCount(language, c, puzzles);
        }

        if (label != null)
        {
            label.text = WordSearchData.LanguageNames[Mathf.Clamp(language, 0, 2)] + " · " + solved + " of " + total;
        }
        if (background != null && hasNormalColor)
        {
            background.color = solved > 0 ? solvedColor : normalColor;
        }
    }
}
