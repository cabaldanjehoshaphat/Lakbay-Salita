using UnityEngine;
using UnityEngine.SceneManagement;

// Centralized scene-navigation helper for the menu flow. Attach one instance to a
// "SceneManager" GameObject in each menu scene and wire UI Button OnClick events to
// its public methods to move between scenes (all scenes must be added to Build Settings).
public class SceneNavigator : MonoBehaviour
{
    [Header("Only needed on the Language Selection scene")]
    [SerializeField] private LanguageSelector languageSelector;
    [SerializeField] private string[] cebuanoWordleScenes;
    [SerializeField] private string[] ilonggoWordleScenes;
    [SerializeField] private string[] tagalogWordleScenes;

    private const string MainMenuScene = "1 Main_menu";
    private const string ProfileIconScene = "2 Profile Icon";
    private const string LanguageSelectionScene = "3 Language_Selection_Menu";
    private const string LibraryMenuScene = "4 Library_menu";

    public void LoadMainMenu() => SceneManager.LoadScene(MainMenuScene);
    public void LoadProfileIcon() => SceneManager.LoadScene(ProfileIconScene);
    public void LoadLanguageSelectionMenu() => SceneManager.LoadScene(LanguageSelectionScene);
    public void LoadLibraryMenu() => SceneManager.LoadScene(LibraryMenuScene);

    // Wired to the Language Selection scene's wordle_button. Loads a random puzzle
    // scene from whichever language radio button is currently checked.
    public void LoadRandomWordlePuzzle()
    {
        if (languageSelector == null)
        {
            Debug.LogWarning("SceneNavigator: languageSelector is not assigned.");
            return;
        }

        string[] pool = languageSelector.SelectedLanguage switch
        {
            PuzzleLanguage.Cebuano => cebuanoWordleScenes,
            PuzzleLanguage.Ilonggo => ilonggoWordleScenes,
            PuzzleLanguage.Tagalog => tagalogWordleScenes,
            _ => null
        };

        if (pool == null || pool.Length == 0)
        {
            Debug.LogWarning("SceneNavigator: no puzzle scenes configured for the selected language.");
            return;
        }

        SceneManager.LoadScene(pool[Random.Range(0, pool.Length)]);
    }
}
