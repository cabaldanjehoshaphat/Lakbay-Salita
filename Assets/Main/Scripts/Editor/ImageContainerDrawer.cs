using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

/// <summary>
/// ImageContainerDrawer
/// Custom Inspector for ImageContainer, mirroring AlphabetDataDrawer
/// (Assets/Sub/Word_Search/Scripts/Editor/AlphabetDataDrawer.cs): one ReorderableList
/// per state (Highlighted/Normal/Plain/Wrong), each row showing a letter field next to
/// its image field. The letter field is drawn as a plain text box (not Unity's default
/// numeric box for char) so you can type an actual letter like "A" instead of an ASCII
/// code. Editor-only — lives in an "Editor" folder so it's excluded from player builds,
/// same as AlphabetDataDrawer.
/// </summary>
[CustomEditor(typeof(ImageContainer))]
[CanEditMultipleObjects]
public class ImageContainerDrawer : Editor
{
    private ReorderableList highlightedList;
    private ReorderableList normalList;
    private ReorderableList plainList;
    private ReorderableList wrongList;

    private void OnEnable()
    {
        InitializeReorderableList(ref highlightedList, "highlighted", "Highlighted");
        InitializeReorderableList(ref normalList, "normal", "Normal");
        InitializeReorderableList(ref plainList, "plain", "Plain");
        InitializeReorderableList(ref wrongList, "wrong", "Wrong");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        highlightedList.DoLayoutList();
        normalList.DoLayoutList();
        plainList.DoLayoutList();
        wrongList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }

    private void InitializeReorderableList(ref ReorderableList list, string propertyName, string listLabel)
    {
        list = new ReorderableList(serializedObject, serializedObject.FindProperty(propertyName),
        true, true, true, true);

        list.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, listLabel);
        };

        var l = list;

        list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = l.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;

            var letterProp = element.FindPropertyRelative("letter");
            var letterRect = new Rect(rect.x, rect.y, 60, EditorGUIUtility.singleLineHeight);
            DrawCharAsTextField(letterRect, letterProp);

            EditorGUI.PropertyField(new Rect(rect.x + 70, rect.y, rect.width - 60 - 30, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("image"), GUIContent.none);
        };
    }

    /// <summary>
    /// Draws a char SerializedProperty as a single-character text field. Unity has no
    /// built-in "charValue" accessor or letter-friendly drawer for char (it defaults to a
    /// plain numeric/ASCII box), so this reads/writes via intValue and converts to/from
    /// a single uppercase character typed as text.
    /// </summary>
    private void DrawCharAsTextField(Rect rect, SerializedProperty charProperty)
    {
        char currentChar = (char)charProperty.intValue;
        string typed = EditorGUI.TextField(rect, currentChar == '\0' ? string.Empty : currentChar.ToString());

        char newChar = string.IsNullOrEmpty(typed) ? '\0' : char.ToUpperInvariant(typed[0]);
        charProperty.intValue = newChar;
    }
}
