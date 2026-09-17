using UnityEngine;
using UnityEngine.UI;

// Which puzzle-word language is currently selected on the Language Selection scene.
public enum PuzzleLanguage
{
    Cebuano,
    Ilonggo,
    Tagalog
}

// Reads whichever language radio button (Toggle) is currently checked so other
// scripts (e.g. SceneNavigator) know which puzzle set to load. The three toggles
// must share one ToggleGroup (set on the "Language_menu" canvas) so only one of
// them can ever be checked at a time.
public class LanguageSelector : MonoBehaviour
{
    [SerializeField] private Toggle cebuanoToggle;
    [SerializeField] private Toggle ilonggoToggle;
    [SerializeField] private Toggle tagalogToggle;

    public PuzzleLanguage SelectedLanguage
    {
        get
        {
            if (cebuanoToggle != null && cebuanoToggle.isOn) return PuzzleLanguage.Cebuano;
            if (ilonggoToggle != null && ilonggoToggle.isOn) return PuzzleLanguage.Ilonggo;
            return PuzzleLanguage.Tagalog;
        }
    }
}
