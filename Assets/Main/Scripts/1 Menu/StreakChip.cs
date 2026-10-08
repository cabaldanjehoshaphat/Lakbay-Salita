using TMPro;
using UnityEngine;

/// <summary>
/// StreakChip
/// Shows the player's current day-streak (from StreakTracker) in the Main Menu's top-left chip.
/// Put it on the chip object and assign the number label (the orange circle's text). The label is refreshed
/// whenever the chip becomes active, so it is always up to date after coming back from a minigame.
/// </summary>
public class StreakChip : MonoBehaviour
{
    [SerializeField] private TMP_Text numberLabel;

    private void OnEnable()
    {
        Refresh();
    }

    /// <summary>Re-reads the saved streak and updates the number.</summary>
    public void Refresh()
    {
        if (numberLabel != null)
        {
            numberLabel.text = StreakTracker.GetStreak().ToString();
        }
    }
}
