using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// WordleHubTag
/// The small tag on the Wordle card of the Language Selection hub. It shows how many Wordle levels the player has solved in the language
/// that is selected on the hub, with the language name in front, for example "Cebuano · 4 of 42" (7 categories x 6 levels), and updates as
/// soon as another language is picked.
/// The tag turns gold once at least one level is solved. Put it on the card's tag (the pill with the Image) and assign the language
/// selector and the text label.
/// </summary>
public class WordleHubTag : MonoBehaviour
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

    /// <summary>Reads the solved levels of the selected language and rewrites the tag.</summary>
    public void Refresh()
    {
        int language = languageSelector != null ? (int)languageSelector.SelectedLanguage : 0;
        shownLanguage = language;

        int total = WordleData.TotalLevels(language);
        int solved = WordleProgressStore.SolvedTotal(language);

        if (label != null)
        {
            label.text = WordleData.LanguageNames[Mathf.Clamp(language, 0, 2)] + " · " + solved + " of " + total;
        }
        if (background != null && hasNormalColor)
        {
            background.color = solved > 0 ? solvedColor : normalColor;
        }
    }
}
