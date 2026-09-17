using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Shows the current puzzle's word definition (from WordleWordVerifier) in the
// Dialog Panel's TMP text. Refreshes automatically in the Editor via OnValidate
// whenever the verifier or its JSON changes, and again on Start at runtime.
public class WordDefinitionDisplay : MonoBehaviour
{
    [SerializeField] private WordleWordVerifier verifier;
    [SerializeField] private TMP_Text definitionText;

    private void Start()
    {
        Refresh();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        Refresh();
    }
#endif

    private void Refresh()
    {
        if (verifier == null || definitionText == null)
        {
            return;
        }

        definitionText.text = verifier.Definition;

        // The Dialog Panel auto-sizes to the wrapped text height (VerticalLayoutGroup +
        // ContentSizeFitter on its parent). At runtime Unity's own Canvas update loop
        // rebuilds that automatically every frame, but in the Editor outside Play mode
        // (e.g. this OnValidate call) nothing drives that rebuild on its own, so the
        // panel would stay the wrong size until something else happened to touch it.
        // Forcing it here — twice, since a nested LayoutGroup/ContentSizeFitter pair
        // needs a second pass to fully converge (first pass fixes width, second
        // recomputes height against that corrected width) — keeps the Inspector preview
        // accurate immediately.
        var panelRect = definitionText.transform.parent as RectTransform;
        if (panelRect != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(panelRect);
            LayoutRebuilder.ForceRebuildLayoutImmediate(panelRect);
        }
    }
}
