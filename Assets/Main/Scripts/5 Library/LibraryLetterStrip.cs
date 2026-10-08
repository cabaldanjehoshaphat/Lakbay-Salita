using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// LibraryLetterStrip
/// Pointer handler on the slim A-Z strip at the right edge of the Library notebook. While the pointer is
/// down or dragging over the strip it reports the vertical position (0 = top, 1 = bottom) to the
/// LibraryNotebook, which jumps to that letter. Lives in its own file so the component can be saved in
/// the scene (a MonoBehaviour needs a script file named after its class).
/// </summary>
public class LibraryLetterStrip : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public LibraryNotebook Owner;

    private void Report(PointerEventData e, bool active)
    {
        var rt = (RectTransform)transform;
        Vector2 local;
        if (Owner != null && RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, e.position, e.pressEventCamera, out local))
        {
            Owner.StripPointer(Mathf.Clamp01(-local.y / rt.rect.height), active);
        }
    }

    public void OnPointerDown(PointerEventData e) { Report(e, true); }
    public void OnDrag(PointerEventData e) { Report(e, true); }

    public void OnPointerUp(PointerEventData e)
    {
        if (Owner != null) Owner.StripPointer(0f, false);
    }
}
