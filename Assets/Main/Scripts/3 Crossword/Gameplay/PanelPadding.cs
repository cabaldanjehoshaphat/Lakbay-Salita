using UnityEngine;

/// <summary>
/// PanelPadding
/// Reusable inner-margin control for any stretch-anchored UI panel (e.g. "crossword panel",
/// "clue panel"). Attach it to a RectTransform whose anchors already stretch to fill its
/// parent, and it insets that rect by the given left/right/top/bottom amounts instead of
/// requiring the offsetMin/offsetMax numbers to be hand-tuned directly. Live-updates via
/// OnValidate, so dragging the values in the Inspector moves the panel's edge immediately.
/// </summary>
[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class PanelPadding : MonoBehaviour
{
    public float left;
    public float right;
    public float top;
    public float bottom;

    private RectTransform _rect;

    private void OnEnable()
    {
        Apply();
    }

    private void OnValidate()
    {
        Apply();
    }

    private void Apply()
    {
        if (_rect == null)
        {
            _rect = GetComponent<RectTransform>();
        }
        _rect.offsetMin = new Vector2(left, bottom);
        _rect.offsetMax = new Vector2(-right, -top);
    }
}
