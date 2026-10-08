using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// MusicToggleButton
/// The round music chip in the main menu's top bar (next to the Profile chip). Tapping it turns the background music off or on through
/// BackgroundMusic.SetMuted, which fades the music and saves the choice for the next launch. The icon shows a music note while music is
/// on and a crossed-out note while it is off; it also updates if the music is muted from anywhere else (BackgroundMusic.MutedChanged).
/// Setup: put this on a GameObject with a Button, assign the child icon Image and the two sprites
/// (Assets/Main/Sprites/1 Menu/Main Menu/main_menu_music_on.png and main_menu_music_off.png).
/// </summary>
[RequireComponent(typeof(Button))]
public class MusicToggleButton : MonoBehaviour
{
    [Tooltip("The Image that shows the music on/off icon.")]
    [SerializeField] private Image icon;

    [Tooltip("Icon shown while music is on.")]
    [SerializeField] private Sprite musicOnSprite;

    [Tooltip("Icon shown while music is off.")]
    [SerializeField] private Sprite musicOffSprite;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(Toggle);
    }

    private void OnEnable()
    {
        BackgroundMusic.MutedChanged += Refresh;
        Refresh(BackgroundMusic.Muted);
    }

    private void OnDisable()
    {
        BackgroundMusic.MutedChanged -= Refresh;
    }

    private void Toggle()
    {
        BackgroundMusic.SetMuted(!BackgroundMusic.Muted);
    }

    private void Refresh(bool muted)
    {
        if (icon != null)
        {
            icon.sprite = muted ? musicOffSprite : musicOnSprite;
        }
    }
}
