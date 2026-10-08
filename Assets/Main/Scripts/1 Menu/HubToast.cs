using TMPro;
using UnityEngine;

/// <summary>
/// HubToast
/// A small pop-up message on the Language Selection hub (for example "Word Search is coming soon").
/// Put it on a GameObject that is inactive by default and assign its text label; calling Show(message)
/// activates it, shows the message, and hides it again after a couple of seconds. Used by
/// SceneNavigator when a minigame has no level scenes configured yet.
/// </summary>
public class HubToast : MonoBehaviour
{
    [SerializeField] private TMP_Text label;
    [SerializeField] private float seconds = 2.2f;

    private float _hideAt;

    public void Show(string message)
    {
        Show(message, seconds);
    }

    /// <summary>Shows the message for a custom number of seconds (for example the Wordle "Get ready" lead-in).</summary>
    public void Show(string message, float visibleSeconds)
    {
        if (label != null)
        {
            label.text = message;
        }
        gameObject.SetActive(true);
        _hideAt = Time.unscaledTime + visibleSeconds;
    }

    private void Update()
    {
        if (Time.unscaledTime >= _hideAt)
        {
            gameObject.SetActive(false);
        }
    }
}
