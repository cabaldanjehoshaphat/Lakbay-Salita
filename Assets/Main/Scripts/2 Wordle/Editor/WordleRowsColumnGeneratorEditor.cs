using UnityEditor;
using UnityEngine;

/// <summary>
/// WordleRowsColumnGeneratorEditor
/// Custom Inspector for WordleRowsColumnGenerator: shows the default fields (rows,
/// columns, spacing, cell prefab, grid parent, cell size) plus a "Generate" button that
/// rebuilds the grid on demand — mirroring the same custom-editor-with-button pattern
/// this project already uses (e.g. BoardDataDrawer's "Fill Up With Random Letters").
/// Editor-only — lives in an "Editor" folder so it's excluded from player builds.
/// </summary>
[CustomEditor(typeof(WordleRowsColumnGenerator))]
public class WordleRowsColumnGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();

        WordleRowsColumnGenerator generator = (WordleRowsColumnGenerator)target;
        if (GUILayout.Button("Generate"))
        {
            generator.Generate();
        }
    }
}
