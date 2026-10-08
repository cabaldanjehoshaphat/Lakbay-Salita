using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// LibraryScrollTrack
/// Pointer handler on the scroll bar track beside the Library notebook page. Clicking or dragging on the
/// track reports the pointer's distance from the track's top (in pixels) to the LibraryNotebook, which
/// scrolls so the handle follows the pointer. Lives in its own file so the component can be saved in the
/// scene (a MonoBehaviour needs a script file named after its class).
/// </summary>
public class LibraryScrollTrack : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public LibraryNotebook Owner;

    private void Report(PointerEventData e, bool active)
    {
        var rt = (RectTransform)transform;
        Vector2 local;
        if (Owner != null && RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, e.position, e.pressEventCamera, out local))
        {
            Owner.TrackPointer(Mathf.Clamp(-local.y, 0f, rt.rect.height), active);
        }
    }

    public void OnPointerDown(PointerEventData e) { Report(e, true); }
    public void OnDrag(PointerEventData e) { Report(e, true); }

    public void OnPointerUp(PointerEventData e)
    {
        if (Owner != null) Owner.TrackPointer(0f, false);
    }
}
