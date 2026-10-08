using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// CrosswordDesignBuilder
/// Editor tool that rebuilds a Crossword puzzle scene in the redesigned look, as real saved scene objects (so the design is
/// visible in the editor before pressing Play). Each scene keeps its own puzzle (language and level on CrosswordInputController,
/// the CrosswordGridGenerator and CrosswordCluePanel components); the old half-screen panels, grid borders and back arrow are
/// replaced by:
///  - the same time-of-day sky as the menus and Wordle (MainMenuTheme),
///  - a top bar (pause button, language, stopwatch, Keypad switch), the active clue card with arrows, the Hint / Check / Reveal tools,
///  - the grid (new rounded cell prefab, black cells are simply left empty), the tabbed Across / Down clue list (scrollable),
///    and the on-screen keypad (hidden until the Keypad switch is turned on),
///  - a toast, the pause window and the win card, all driven by CrosswordGameController.
/// It creates its own prefabs (CrosswordCellV2, CrosswordClueRowV2, CrosswordBlackCellV2) and text style (CrosswordTextStyleV2) so the
/// original ones stay untouched. Menu: Tools > Crossword > "Rebuild design in open scene" or "Rebuild design in all 30 level scenes".
/// Safe to run again: it deletes and recreates only its own objects. The 1st-design scenes are archived in
/// Assets/Main/Scenes/3 Crossword/Archive.
/// </summary>
public static class CrosswordDesignBuilder
{
    private const string PrefabFolder = "Assets/Main/Prefabs/3 Crossword";
    private const string CellPrefabPath = PrefabFolder + "/CrosswordCellV2.prefab";
    private const string RowPrefabPath = PrefabFolder + "/CrosswordClueRowV2.prefab";
    private const string BlackPrefabPath = PrefabFolder + "/CrosswordBlackCellV2.prefab";
    private const string StylePath = "Assets/Main/Data/Crossword/Settings/CrosswordTextStyleV2.asset";
    private const string MaterialPath = "Assets/Main/Sprites/1 Menu/Main Menu/LiberationSans SDF Colorable.mat";
    private const string SpriteFolder = "Assets/Main/Sprites/1 Menu/Main Menu/";

    private static readonly Color Cream = new Color32(253, 248, 238, 255);
    private static readonly Color Ink = new Color32(59, 42, 26, 255);
    private static readonly Color InkSoft = new Color32(107, 82, 55, 255);
    private static readonly Color Green = new Color32(93, 160, 42, 255);
    private static readonly Color Yellow = new Color32(232, 185, 35, 255);
    private static readonly Color Purple = new Color32(138, 63, 199, 255);
    private static readonly Color Navy = new Color32(27, 58, 140, 255);
    private static readonly Color Orange = new Color32(232, 117, 26, 255);
    private static readonly Color Sand = new Color32(239, 231, 212, 255);

    private static Material _mat;

    // ------------------------------------------------------------------ menu entries

    [MenuItem("Tools/Crossword/Rebuild design in open scene")]
    public static void BuildOpenScene()
    {
        Debug.Log(BuildCurrentScene());
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }

    [MenuItem("Tools/Crossword/Rebuild design in all 30 level scenes")]
    public static void BuildAllMenu()
    {
        Debug.Log(BuildRange(0, 30));
    }

    /// <summary>Rebuilds levels start..start+count-1 (0-29: Cebuano 1-10, Ilonggo 1-10, Tagalog 1-10), saving each scene.</summary>
    public static string BuildRange(int start, int count)
    {
        var sb = new StringBuilder();
        string[] langs = { "1 - Puzzle - Cebuano|Cebuano", "2 - Puzzle - Ilonggo|Ilonggo", "3 - Puzzle - Tagalog|Tagalog" };
        for (int i = start; i < start + count && i < 30; i++)
        {
            string[] parts = langs[i / 10].Split('|');
            int level = (i % 10) + 1;
            string path = "Assets/Main/Scenes/3 Crossword/" + parts[0] + "/puzzle-crossword-generator " + level + " " + parts[1] + ".unity";
            var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            string result = BuildCurrentScene();
            EditorSceneManager.SaveScene(scene);
            sb.AppendLine(scene.name + ": " + result);
        }
        return sb.ToString();
    }

    // ------------------------------------------------------------------ assets

    private static Material Mat()
    {
        if (_mat == null)
        {
            _mat = AssetDatabase.LoadAssetAtPath<Material>(MaterialPath);
        }
        return _mat;
    }

    private static Sprite Spr(string file)
    {
        return AssetDatabase.LoadAssetAtPath<Sprite>(SpriteFolder + file);
    }

    private static Image NewImage(string name, Transform parent, Color color, bool rounded)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.layer = 5;
        go.transform.SetParent(parent, false);
        var img = go.AddComponent<Image>();
        if (rounded)
        {
            img.sprite = LibraryUiKit.Rounded;
            img.type = Image.Type.Sliced;
            img.pixelsPerUnitMultiplier = 3f;
        }
        img.color = color;
        img.raycastTarget = false;
        return img;
    }

    private static TextMeshProUGUI NewText(string name, Transform parent, float size, Color color, FontStyles style, TextAlignmentOptions align)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.layer = 5;
        go.transform.SetParent(parent, false);
        var t = go.AddComponent<TextMeshProUGUI>();
        t.text = string.Empty;
        t.fontSize = size;
        t.fontStyle = style;
        t.alignment = align;
        t.color = color;
        t.raycastTarget = false;
        t.enableWordWrapping = false;
        t.fontSharedMaterial = Mat();
        return t;
    }

    private static void Stretch(RectTransform r, float left, float bottom, float right, float top)
    {
        r.anchorMin = Vector2.zero;
        r.anchorMax = Vector2.one;
        r.pivot = new Vector2(0.5f, 0.5f);
        r.offsetMin = new Vector2(left, bottom);
        r.offsetMax = new Vector2(-right, -top);
    }

    /// <summary>Creates the cell, clue row and black cell prefabs and the text style asset (only the ones that do not exist yet).</summary>
    public static void EnsureAssets()
    {
        if (!AssetDatabase.IsValidFolder(PrefabFolder))
        {
            AssetDatabase.CreateFolder("Assets/Main/Prefabs", "3 Crossword");
        }

        if (AssetDatabase.LoadAssetAtPath<CrosswordTextStyle>(StylePath) == null)
        {
            var style = ScriptableObject.CreateInstance<CrosswordTextStyle>();
            style.letterFontSize = 40f;
            style.letterFontStyle = FontStyles.Bold;
            style.letterColor = Ink;
            style.cellNumberFontSize = 16f;
            style.cellNumberColor = InkSoft;
            style.borderColor = Ink;
            style.borderOpacity = 1f;
            style.borderThickness = 3f;
            style.clueNumberFontSize = 32f;
            style.clueNumberColor = Ink;
            style.clueTextFontSize = 30f;
            style.clueTextColor = Ink;
            AssetDatabase.CreateAsset(style, StylePath);
        }

        if (AssetDatabase.LoadAssetAtPath<GameObject>(BlackPrefabPath) == null)
        {
            var black = new GameObject("CrosswordBlackCellV2", typeof(RectTransform));
            black.layer = 5;
            var br = (RectTransform)black.transform;
            br.anchorMin = br.anchorMax = new Vector2(0.5f, 0.5f);
            br.pivot = new Vector2(0.5f, 0.5f);
            br.sizeDelta = new Vector2(60f, 60f);
            PrefabUtility.SaveAsPrefabAsset(black, BlackPrefabPath);
            Object.DestroyImmediate(black);
        }

        // the cell prefab is always rewritten (same path, so existing references stay valid)
        {
            var root = new GameObject("CrosswordCellV2", typeof(RectTransform));
            root.layer = 5;
            var rr = (RectTransform)root.transform;
            rr.anchorMin = rr.anchorMax = new Vector2(0.5f, 0.5f);
            rr.pivot = new Vector2(0.5f, 0.5f);
            rr.sizeDelta = new Vector2(60f, 60f);

            var border = root.AddComponent<Image>();
            border.sprite = LibraryUiKit.Rounded;
            border.type = Image.Type.Sliced;
            border.pixelsPerUnitMultiplier = 3f;
            border.color = Ink;
            border.raycastTarget = true;

            Image fill = NewImage("Fill", root.transform, Cream, true);
            Stretch(fill.rectTransform, 3f, 3f, 3f, 3f);

            TextMeshProUGUI letter = NewText("Letter", fill.transform, 40f, Ink, FontStyles.Bold, TextAlignmentOptions.Center);
            letter.enableAutoSizing = true;
            letter.fontSizeMin = 12f;
            letter.fontSizeMax = 36f;
            letter.overflowMode = TextOverflowModes.Overflow;
            Stretch(letter.rectTransform, 2f, 2f, 2f, 9f);

            TextMeshProUGUI number = NewText("Number", fill.transform, 16f, InkSoft, FontStyles.Normal, TextAlignmentOptions.TopLeft);
            number.enableAutoSizing = true;
            number.fontSizeMin = 8f;
            number.fontSizeMax = 15f;
            number.overflowMode = TextOverflowModes.Overflow;
            number.rectTransform.anchorMin = new Vector2(0f, 0.5f);
            number.rectTransform.anchorMax = new Vector2(0.6f, 1f);
            number.rectTransform.pivot = new Vector2(0f, 1f);
            number.rectTransform.offsetMin = new Vector2(4f, 0f);
            number.rectTransform.offsetMax = new Vector2(0f, -1f);

            var cell = root.AddComponent<CrosswordCell>();
            var so = new SerializedObject(cell);
            SetRef(so, "border", border);
            SetRef(so, "fill", fill);
            SetRef(so, "fillRect", fill.rectTransform);
            SetRef(so, "letterText", letter);
            SetRef(so, "numberText", number);
            so.FindProperty("normalColor").colorValue = Cream;
            so.FindProperty("activeWordColor").colorValue = new Color32(207, 232, 255, 255);
            so.FindProperty("activeCellColor").colorValue = Color.white;
            so.FindProperty("solvedColor").colorValue = new Color32(143, 208, 90, 255);
            so.FindProperty("solvedActiveCellColor").colorValue = Green;
            so.FindProperty("cursorShowsBorder").boolValue = true;
            so.FindProperty("cursorBorderColor").colorValue = new Color32(245, 166, 35, 255);
            so.FindProperty("wrongColor").colorValue = new Color32(247, 193, 193, 255);
            so.ApplyModifiedPropertiesWithoutUndo();

            PrefabUtility.SaveAsPrefabAsset(root, CellPrefabPath);
            Object.DestroyImmediate(root);
        }

        if (AssetDatabase.LoadAssetAtPath<GameObject>(RowPrefabPath) == null)
        {
            var root = new GameObject("CrosswordClueRowV2", typeof(RectTransform));
            root.layer = 5;
            var rr = (RectTransform)root.transform;
            rr.anchorMin = rr.anchorMax = new Vector2(0f, 1f);
            rr.pivot = new Vector2(0f, 1f);
            rr.sizeDelta = new Vector2(1000f, 64f);

            var bg = root.AddComponent<Image>();
            bg.sprite = LibraryUiKit.Rounded;
            bg.type = Image.Type.Sliced;
            bg.pixelsPerUnitMultiplier = 3f;
            bg.color = new Color(0f, 0f, 0f, 0f);
            bg.raycastTarget = true;

            TextMeshProUGUI number = NewText("Number", root.transform, 32f, Ink, FontStyles.Bold, TextAlignmentOptions.MidlineLeft);
            number.rectTransform.anchorMin = new Vector2(0f, 0f);
            number.rectTransform.anchorMax = new Vector2(0f, 1f);
            number.rectTransform.pivot = new Vector2(0f, 0.5f);
            number.rectTransform.offsetMin = new Vector2(14f, 0f);
            number.rectTransform.offsetMax = new Vector2(86f, 0f);

            TextMeshProUGUI clue = NewText("Clue", root.transform, 30f, Ink, FontStyles.Normal, TextAlignmentOptions.MidlineLeft);
            clue.enableWordWrapping = true;
            Stretch(clue.rectTransform, 92f, 4f, 64f, 4f);

            Image done = NewImage("Done", root.transform, Green, false);
            done.sprite = LibraryUiKit.Circle;
            done.rectTransform.anchorMin = done.rectTransform.anchorMax = new Vector2(1f, 0.5f);
            done.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            done.rectTransform.sizeDelta = new Vector2(40f, 40f);
            done.rectTransform.anchoredPosition = new Vector2(-30f, 0f);
            Image armShort = NewImage("check_a", done.transform, Color.white, false);
            armShort.rectTransform.sizeDelta = new Vector2(12f, 4f);
            armShort.rectTransform.anchoredPosition = new Vector2(-5f, -4f);
            armShort.rectTransform.localRotation = Quaternion.Euler(0f, 0f, -45f);
            Image armLong = NewImage("check_b", done.transform, Color.white, false);
            armLong.rectTransform.sizeDelta = new Vector2(22f, 4f);
            armLong.rectTransform.anchoredPosition = new Vector2(4f, 0f);
            armLong.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 50f);
            done.gameObject.SetActive(false);

            var row = root.AddComponent<CrosswordClueRow>();
            var so = new SerializedObject(row);
            SetRef(so, "numberText", number);
            SetRef(so, "clueText", clue);
            SetRef(so, "background", bg);
            SetRef(so, "completedBadge", done.gameObject);
            so.FindProperty("normalColor").colorValue = new Color(0f, 0f, 0f, 0f);
            so.FindProperty("activeColor").colorValue = new Color32(207, 232, 255, 255);
            so.FindProperty("completedColor").colorValue = new Color32(95, 94, 90, 255);
            so.FindProperty("completedBackgroundColor").colorValue = new Color32(234, 243, 222, 255);
            so.FindProperty("verticalPadding").floatValue = 16f;
            so.ApplyModifiedPropertiesWithoutUndo();

            PrefabUtility.SaveAsPrefabAsset(root, RowPrefabPath);
            Object.DestroyImmediate(root);
        }

        AssetDatabase.SaveAssets();
    }

    // ------------------------------------------------------------------ small UI helpers

    private static TextMeshProUGUI Lab(Transform parent, string text, float size, Color color, bool bold,
        TextAlignmentOptions align, float x, float y, float w, float h)
    {
        TextMeshProUGUI t = LibraryUiKit.Label(parent, text, size, color, bold ? FontStyles.Bold : FontStyles.Normal, align);
        t.fontSharedMaterial = Mat();
        LibraryUiKit.Place(t.rectTransform, x, y, w, h);
        return t;
    }

    private static Image Pill(Transform parent, string name, string text, float x, float y, float w, float h, Color bg, Color fg,
        float fontSize, bool bold, out TextMeshProUGUI label)
    {
        Image img = LibraryUiKit.Box(parent, name, bg, true);
        LibraryUiKit.Place(img.rectTransform, x, y, w, h);
        label = Lab(img.transform, text, fontSize, fg, bold, TextAlignmentOptions.Center, 0, 0, w, h);
        LibraryUiKit.Fill(label.rectTransform);
        return img;
    }

    private static Button MakeButton(Image target)
    {
        Button b = LibraryUiKit.MakeButton(target, null);
        b.transition = Selectable.Transition.ColorTint;
        var colors = b.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = Color.white;
        colors.pressedColor = new Color(0.82f, 0.82f, 0.82f, 1f);
        colors.selectedColor = Color.white;
        colors.disabledColor = new Color(1f, 1f, 1f, 0.5f);
        colors.fadeDuration = 0.08f;
        b.colors = colors;
        b.navigation = new Navigation { mode = Navigation.Mode.None };
        return b;
    }

    private static CanvasGroup NewGroup(Transform parent, string name)
    {
        RectTransform r = LibraryUiKit.NewRect(name, parent);
        LibraryUiKit.Fill(r);
        var cg = r.gameObject.AddComponent<CanvasGroup>();
        cg.interactable = false;
        cg.blocksRaycasts = false;
        return cg;
    }

    private static void SetRef(SerializedObject so, string name, Object value)
    {
        SerializedProperty p = so.FindProperty(name);
        if (p == null)
        {
            Debug.LogWarning("CrosswordDesignBuilder: missing field " + name);
            return;
        }
        p.objectReferenceValue = value;
    }

    private static void SetArray<T>(SerializedObject so, string name, IList<T> items) where T : Object
    {
        SerializedProperty p = so.FindProperty(name);
        if (p == null)
        {
            Debug.LogWarning("CrosswordDesignBuilder: missing array " + name);
            return;
        }
        p.arraySize = items.Count;
        for (int i = 0; i < items.Count; i++)
        {
            p.GetArrayElementAtIndex(i).objectReferenceValue = items[i];
        }
    }

    private static void DestroyNamed(Transform parent, string name)
    {
        Transform t;
        while ((t = parent.Find(name)) != null)
        {
            t.SetParent(null, false);
            Object.DestroyImmediate(t.gameObject);
        }
    }

    private static WordleKeyButton MakeKey(Transform parent, string name, WordleKeyKind kind, char letter, string text,
        float x, float y, float w, float h, Color bg, Color fg, float fontSize)
    {
        Image img = LibraryUiKit.Box(parent, name, bg, true);
        LibraryUiKit.Place(img.rectTransform, x, y, w, h);
        TextMeshProUGUI label = Lab(img.transform, text, fontSize, fg, true, TextAlignmentOptions.Center, 0, 0, w, h);
        LibraryUiKit.Fill(label.rectTransform);
        MakeButton(img);
        var key = img.gameObject.AddComponent<WordleKeyButton>();
        key.kind = kind;
        key.letter = letter;
        key.background = img;
        key.label = label;
        return key;
    }

    private static void MakeStat(Transform parent, string name, string caption, float x, float w, Color bg, Color valueColor,
        out TextMeshProUGUI value)
    {
        Image pill = LibraryUiKit.Box(parent, name, bg, true);
        LibraryUiKit.Place(pill.rectTransform, x, 0, w, 92);
        Lab(pill.transform, caption, 28, InkSoft, false, TextAlignmentOptions.Center, 0, 4, w, 36);
        value = Lab(pill.transform, "-", 40, valueColor, true, TextAlignmentOptions.Center, 0, 38, w, 50);
    }

    // ------------------------------------------------------------------ the scene build

    /// <summary>Rebuilds the design in the currently open Crossword puzzle scene. Returns a one-line summary.</summary>
    public static string BuildCurrentScene()
    {
        var canvasGo = GameObject.Find("Canvas");
        var panelGo = GameObject.Find("crossword panel");
        var input = Object.FindObjectOfType<CrosswordInputController>();
        var cluePanel = Object.FindObjectOfType<CrosswordCluePanel>();
        if (canvasGo == null || panelGo == null || input == null || cluePanel == null)
        {
            return "SKIPPED (not a crossword puzzle scene)";
        }
        Transform canvas = canvasGo.transform;
        var generator = panelGo.GetComponent<CrosswordGridGenerator>();
        EnsureAssets();
        Material mat = Mat();
        string sceneName = canvasGo.scene.name;
        string language = sceneName.Contains("Cebuano") ? "Cebuano" : (sceneName.Contains("Tagalog") ? "Tagalog" : "Ilonggo");

        // ---- clean up the old design and any earlier run of this builder
        if (panelGo.transform.parent != canvas)
        {
            panelGo.transform.SetParent(canvas, false);
        }
        DestroyNamed(canvas, "CrosswordUI");
        DestroyNamed(canvas, "CrosswordOverlays");
        DestroyNamed(canvas, "CrosswordController");
        DestroyNamed(canvas, "CrosswordTheme");
        foreach (string old in new[] { "Across", "Down", "Grid Border Top", "Grid Border Bottom", "Grid Border Left", "Grid Border Right", "back_to_main_menu_button" })
        {
            DestroyNamed(canvas, old);
        }

        // ---- canvas scaler (same as the menus)
        var scaler = canvasGo.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

        // ---- sky base (the old background object becomes the solid sky colour)
        Transform bgT = canvas.Find("Backround");
        if (bgT == null) bgT = canvas.Find("sky_base");
        var bg = bgT.GetComponent<Image>();
        bg.gameObject.name = "sky_base";
        bg.sprite = null;
        bg.color = new Color32(95, 180, 236, 255);
        bg.raycastTarget = false;
        bg.rectTransform.SetSiblingIndex(0);

        RectTransform ui = LibraryUiKit.NewRect("CrosswordUI", canvas);
        LibraryUiKit.Fill(ui);
        ui.SetSiblingIndex(1);

        // ---- sky layers
        Image skyTop = LibraryUiKit.Icon(ui, "sky_top", Spr("main_menu_sky_gradient.png"), new Color32(191, 230, 255, 255));
        LibraryUiKit.Fill(skyTop.rectTransform);

        CanvasGroup nightG = NewGroup(ui, "night_group");
        var rnd = new System.Random(21);
        for (int i = 0; i < 40; i++)
        {
            float sx = rnd.Next(20, 1900), sy = rnd.Next(150, 1040), sz = rnd.Next(4, 10);
            Image star = LibraryUiKit.Icon(nightG.transform, "star", LibraryUiKit.Circle, new Color(1f, 1f, 0.92f, 0.8f));
            LibraryUiKit.Place(star.rectTransform, sx, sy, sz, sz);
        }
        Image moon = LibraryUiKit.Icon(nightG.transform, "moon", LibraryUiKit.Circle, new Color32(255, 243, 196, 255));
        LibraryUiKit.Place(moon.rectTransform, 1130, 36, 96, 96);

        CanvasGroup sunG = NewGroup(ui, "sun_group");
        Image sunHalo = LibraryUiKit.Icon(sunG.transform, "sun_halo", LibraryUiKit.Circle, new Color(1f, 0.96f, 0.78f, 0.43f));
        LibraryUiKit.Place(sunHalo.rectTransform, 720, -220, 480, 480);
        Image sunCore = LibraryUiKit.Icon(sunG.transform, "sun_core", LibraryUiKit.Circle, new Color(1f, 0.93f, 0.67f, 0.9f));
        LibraryUiKit.Place(sunCore.rectTransform, 820, -120, 280, 280);

        CanvasGroup cloudG = NewGroup(ui, "clouds_group");
        float[][] cloudDefs = { new[] { 830f, 34f, 0.9f }, new[] { 1240f, 60f, 0.7f } };
        float[][] puffs = { new[] { 0f, 10f, 60f, 24f }, new[] { 14f, 0f, 40f, 30f }, new[] { 36f, 6f, 50f, 26f }, new[] { -10f, 14f, 40f, 18f } };
        for (int i = 0; i < cloudDefs.Length; i++)
        {
            RectTransform cr = LibraryUiKit.NewRect("cloud_" + (i + 1), cloudG.transform);
            LibraryUiKit.Place(cr, cloudDefs[i][0], cloudDefs[i][1], 10, 10);
            float s = cloudDefs[i][2] * 2f;
            foreach (float[] pf in puffs)
            {
                Image pu = LibraryUiKit.Icon(cr, "puff", LibraryUiKit.Circle, new Color(1, 1, 1, 0.82f));
                LibraryUiKit.Place(pu.rectTransform, pf[0] * s, pf[1] * s, pf[2] * s, pf[3] * s);
            }
        }

        // ---- board group: grid, clue card, tools, clue list, keypad
        CanvasGroup board = NewGroup(ui, "Board");
        board.interactable = true;
        board.blocksRaycasts = true;

        // grid area (the object that already holds CrosswordGridGenerator)
        var panelRect = (RectTransform)panelGo.transform;
        panelRect.SetParent(board.transform, false);
        LibraryUiKit.Place(panelRect, 60, 156, 640, 900);
        panelRect.localScale = Vector3.one;
        var panelImage = panelGo.GetComponent<Image>();
        if (panelImage != null)
        {
            Object.DestroyImmediate(panelImage);
        }
        var genCanvasRenderer = panelGo.GetComponent<CanvasRenderer>();
        panelRect.SetSiblingIndex(0);

        // clue card
        Image clueCard = LibraryUiKit.Box(board.transform, "clue_card", Cream, true);
        LibraryUiKit.Place(clueCard.rectTransform, 700, 164, 1160, 192);
        TextMeshProUGUI clueLabel = Lab(clueCard.transform, "1 Across", 32, InkSoft, false, TextAlignmentOptions.TopLeft, 24, 14, 920, 40);
        TextMeshProUGUI clueText = Lab(clueCard.transform, "", 40, Ink, false, TextAlignmentOptions.TopLeft, 24, 56, 920, 124);
        clueText.enableWordWrapping = true;
        clueText.enableAutoSizing = true;
        clueText.fontSizeMin = 26f;
        clueText.fontSizeMax = 40f;
        Image prevImg = LibraryUiKit.Icon(clueCard.transform, "previous_button", LibraryUiKit.Circle, Sand);
        LibraryUiKit.Place(prevImg.rectTransform, 972, 62, 68, 68);
        Image prevArrow = LibraryUiKit.Icon(prevImg.transform, "arrow", Spr("main_menu_play_triangle.png"), Ink);
        LibraryUiKit.Place(prevArrow.rectTransform, 20, 18, 28, 32);
        prevArrow.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        prevArrow.rectTransform.anchoredPosition = new Vector2(20f + 14f, -(18f + 16f));
        prevArrow.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 180f);
        Button prevButton = MakeButton(prevImg);
        Image nextImg = LibraryUiKit.Icon(clueCard.transform, "next_button", LibraryUiKit.Circle, Sand);
        LibraryUiKit.Place(nextImg.rectTransform, 1060, 62, 68, 68);
        Image nextArrow = LibraryUiKit.Icon(nextImg.transform, "arrow", Spr("main_menu_play_triangle.png"), Ink);
        LibraryUiKit.Place(nextArrow.rectTransform, 22, 18, 28, 32);
        Button nextButton = MakeButton(nextImg);

        // tools
        TextMeshProUGUI hintLabel, checkLabel, revealLabel;
        Image hintImg = Pill(board.transform, "hint_button", "Hint · 3", 700, 380, 224, 68, Yellow, Ink, 34, true, out hintLabel);
        Button hintButton = MakeButton(hintImg);
        Image checkImg = Pill(board.transform, "check_button", "Check", 944, 380, 224, 68, Cream, Ink, 34, false, out checkLabel);
        Button checkButton = MakeButton(checkImg);
        Image revealImg = Pill(board.transform, "reveal_button", "Reveal · 1", 1188, 380, 224, 68, Cream, Ink, 34, false, out revealLabel);
        Button revealButton = MakeButton(revealImg);

        // clue list card with tabs and two scroll lists
        Image listCard = LibraryUiKit.Box(board.transform, "list_card", Cream, true);
        LibraryUiKit.Place(listCard.rectTransform, 700, 468, 1160, 592);
        TextMeshProUGUI acrossTabLabel, downTabLabel;
        Image acrossTabImg = Pill(listCard.transform, "across_tab", "Across", 28, 12, 240, 60, Navy, Color.white, 34, true, out acrossTabLabel);
        Button acrossTabButton = MakeButton(acrossTabImg);
        Image downTabImg = Pill(listCard.transform, "down_tab", "Down", 284, 12, 240, 60, Sand, Ink, 34, true, out downTabLabel);
        Button downTabButton = MakeButton(downTabImg);

        RectTransform acrossContent, downContent;
        ScrollRect acrossScroll = LibraryUiKit.MakeScroll(listCard.transform, "across_scroll", 14, 84, 1132, 494, out acrossContent);
        acrossContent.name = "across_list";
        ScrollRect downScroll = LibraryUiKit.MakeScroll(listCard.transform, "down_scroll", 14, 84, 1132, 494, out downContent);
        downContent.name = "down_list";

        // keypad (hidden until the Keypad switch is on)
        RectTransform keypad = LibraryUiKit.NewRect("Keypad", board.transform);
        LibraryUiKit.Fill(keypad);
        var keys = new List<WordleKeyButton>();
        string[] rows = { "QWERTYUIOP", "ASDFGHJKL", "ZXCVBNM" };
        float[] rowY = { 776f, 864f, 952f };
        for (int r = 0; r < 3; r++)
        {
            string letters = rows[r];
            bool last = r == 2;
            float width = letters.Length * 104f + (letters.Length - 1) * 12f + (last ? 12f + 152f : 0f);
            float x = 700f + (1160f - width) / 2f;
            foreach (char c in letters)
            {
                keys.Add(MakeKey(keypad, "key_" + c, WordleKeyKind.Letter, c, c.ToString(), x, rowY[r], 104f, 76f, Cream, Ink, 38f));
                x += 104f + 12f;
            }
            if (last)
            {
                WordleKeyButton back = MakeKey(keypad, "key_backspace", WordleKeyKind.Backspace, '\b', "", x, rowY[r], 152f, 76f, Cream, Ink, 30f);
                Image bar = LibraryUiKit.Box(back.transform, "bar", Ink, true);
                LibraryUiKit.Place(bar.rectTransform, 62, 34, 52, 8);
                Image head = LibraryUiKit.Icon(back.transform, "arrow_head", Spr("main_menu_play_triangle.png"), Ink);
                LibraryUiKit.Place(head.rectTransform, 36, 24, 26, 28);
                head.rectTransform.pivot = new Vector2(0.5f, 0.5f);
                head.rectTransform.anchoredPosition = new Vector2(36f + 13f, -(24f + 14f));
                head.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 180f);
                keys.Add(back);
            }
        }
        keypad.gameObject.SetActive(false);

        // ---- top bar
        Image pauseRing = LibraryUiKit.Icon(ui, "pause_button", LibraryUiKit.Circle, Orange);
        LibraryUiKit.Place(pauseRing.rectTransform, 48, 40, 112, 112);
        Image pauseInner = LibraryUiKit.Icon(pauseRing.transform, "inner", LibraryUiKit.Circle, Cream);
        LibraryUiKit.Place(pauseInner.rectTransform, 6, 6, 100, 100);
        Image bar1 = LibraryUiKit.Box(pauseRing.transform, "bar_1", Orange, true);
        LibraryUiKit.Place(bar1.rectTransform, 38, 30, 14, 52);
        Image bar2 = LibraryUiKit.Box(pauseRing.transform, "bar_2", Orange, true);
        LibraryUiKit.Place(bar2.rectTransform, 60, 30, 14, 52);
        Button pauseButton = MakeButton(pauseRing);

        TextMeshProUGUI langLabel;
        Pill(ui, "language_chip", language, 230, 60, 224, 64, Cream, Ink, 36, false, out langLabel);

        Image timerPill = LibraryUiKit.Box(ui, "timer_pill", Cream, true);
        LibraryUiKit.Place(timerPill.rectTransform, 480, 48, 300, 88);
        Image clock = LibraryUiKit.Icon(timerPill.transform, "clock", LibraryUiKit.Circle, new Color32(143, 208, 90, 255));
        LibraryUiKit.Place(clock.rectTransform, 22, 16, 56, 56);
        Image handA = LibraryUiKit.Box(clock.transform, "hand_a", new Color32(39, 80, 10, 255), false);
        LibraryUiKit.Place(handA.rectTransform, 26, 10, 5, 20);
        Image handB = LibraryUiKit.Box(clock.transform, "hand_b", new Color32(39, 80, 10, 255), false);
        LibraryUiKit.Place(handB.rectTransform, 26, 25, 16, 5);
        TextMeshProUGUI timerLabel = Lab(timerPill.transform, "00:00", 52, Ink, true, TextAlignmentOptions.Center, 90, 0, 190, 88);

        Image keypadPill = LibraryUiKit.Box(ui, "keypad_switch", Cream, true);
        LibraryUiKit.Place(keypadPill.rectTransform, 1500, 48, 380, 88);
        Lab(keypadPill.transform, "Keypad", 36, InkSoft, false, TextAlignmentOptions.MidlineLeft, 28, 0, 170, 88);
        Image track = LibraryUiKit.Box(keypadPill.transform, "track", new Color32(185, 183, 172, 255), true);
        LibraryUiKit.Place(track.rectTransform, 216, 22, 120, 44);
        Image knob = LibraryUiKit.Icon(track.transform, "knob", LibraryUiKit.Circle, Color.white);
        LibraryUiKit.Place(knob.rectTransform, 4, 4, 36, 36);
        keypadPill.raycastTarget = true;
        var keypadToggle = keypadPill.gameObject.AddComponent<Toggle>();
        keypadToggle.targetGraphic = keypadPill;
        keypadToggle.transition = Selectable.Transition.None;
        keypadToggle.navigation = new Navigation { mode = Navigation.Mode.None };
        keypadToggle.isOn = false;

        // ---- toast
        Image toastBox = LibraryUiKit.Box(ui, "toast", new Color(0.23f, 0.16f, 0.1f, 0.94f), true);
        LibraryUiKit.Place(toastBox.rectTransform, 800, 56, 680, 72);
        TextMeshProUGUI toastLabel = Lab(toastBox.transform, "", 32, Cream, false, TextAlignmentOptions.Center, 0, 0, 680, 72);
        LibraryUiKit.Fill(toastLabel.rectTransform);
        var toast = toastBox.gameObject.AddComponent<HubToast>();
        var toastSo = new SerializedObject(toast);
        SetRef(toastSo, "label", toastLabel);
        toastSo.ApplyModifiedPropertiesWithoutUndo();
        toastBox.gameObject.SetActive(false);

        // ---- sky theme (time of day)
        RectTransform themeRect = LibraryUiKit.NewRect("CrosswordTheme", canvas);
        LibraryUiKit.Fill(themeRect);
        var theme = themeRect.gameObject.AddComponent<MainMenuTheme>();
        var themeSo = new SerializedObject(theme);
        SetRef(themeSo, "skyBase", bg);
        SetRef(themeSo, "skyTopOverlay", skyTop);
        SetRef(themeSo, "clouds", cloudG);
        SetRef(themeSo, "sun", sunG);
        SetRef(themeSo, "night", nightG);
        themeSo.ApplyModifiedPropertiesWithoutUndo();
        theme.LoadDefaultThemes();
        theme.PreviewPeriod(DayPeriod.Noon);

        // ---- overlays: win card and pause window
        RectTransform overlays = LibraryUiKit.NewRect("CrosswordOverlays", canvas);
        LibraryUiKit.Fill(overlays);

        Image resultDim = LibraryUiKit.Box(overlays, "result_panel", new Color(0.07f, 0.16f, 0.36f, 0.55f), false);
        LibraryUiKit.Fill(resultDim.rectTransform);
        resultDim.raycastTarget = true;
        RectTransform confetti = LibraryUiKit.NewRect("confetti", resultDim.transform);
        LibraryUiKit.Fill(confetti);
        Color[] confettiColors = { Yellow, Green, Orange, Purple };
        var crnd = new System.Random(5);
        for (int i = 0; i < 18; i++)
        {
            Image piece = LibraryUiKit.Box(confetti, "piece_" + i, confettiColors[i % 4], true);
            LibraryUiKit.Place(piece.rectTransform, crnd.Next(80, 1840), crnd.Next(0, 1000), 28, 16);
            piece.rectTransform.localRotation = Quaternion.Euler(0f, 0f, crnd.Next(0, 180));
        }
        Image card = LibraryUiKit.Box(resultDim.transform, "card", Cream, true);
        LibraryUiKit.Place(card.rectTransform, 400, 120, 1120, 920);
        card.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        card.rectTransform.anchoredPosition = new Vector2(400 + 560, -(120 + 460));
        TextMeshProUGUI resultTitle;
        Pill(card.transform, "ribbon", "Husto! Puzzle done", 200, -48, 720, 108, Navy, Color.white, 52, true, out resultTitle);
        Lab(card.transform, "Words you learned", 36, InkSoft, false, TextAlignmentOptions.Center, 0, 84, 1120, 50);

        RectTransform learnedContent;
        ScrollRect learnedScroll = LibraryUiKit.MakeScroll(card.transform, "learned_scroll", 56, 140, 1008, 500, out learnedContent);
        learnedContent.name = "learned_list";
        var vlg = learnedContent.gameObject.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 8f;
        vlg.childControlWidth = true;
        vlg.childControlHeight = false;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
        var fitter = learnedContent.gameObject.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        Image rowTemplate = LibraryUiKit.Box(learnedContent, "RowTemplate", Sand, true);
        rowTemplate.rectTransform.sizeDelta = new Vector2(0f, 72f);
        Lab(rowTemplate.transform, "WORD", 38, Ink, true, TextAlignmentOptions.MidlineLeft, 24, 0, 230, 72).name = "Word";
        TextMeshProUGUI meaning = Lab(rowTemplate.transform, "meaning", 30, InkSoft, false, TextAlignmentOptions.MidlineLeft, 262, 0, 640, 72);
        meaning.name = "Meaning";
        meaning.enableWordWrapping = true;
        meaning.enableAutoSizing = true;
        meaning.fontSizeMin = 20f;
        meaning.fontSizeMax = 30f;
        Image starIcon = LibraryUiKit.Icon(rowTemplate.transform, "Star", LibraryUiKit.Star, new Color32(185, 183, 172, 255));
        LibraryUiKit.Place(starIcon.rectTransform, 930, 12, 48, 48);
        MakeButton(starIcon);
        rowTemplate.gameObject.SetActive(false);

        Lab(card.transform, "Tap a star to save a word to your favorites", 28, InkSoft, false, TextAlignmentOptions.Center, 0, 652, 1120, 40);

        RectTransform stats = LibraryUiKit.NewRect("stats", card.transform);
        LibraryUiKit.Place(stats, 56, 704, 1008, 92);
        TextMeshProUGUI statTime, statHints, statStreak;
        MakeStat(stats, "stat_time", "Time", 0, 300, Sand, Ink, out statTime);
        MakeStat(stats, "stat_hints", "Hints used", 334, 340, Sand, Ink, out statHints);
        MakeStat(stats, "stat_streak", "Streak", 708, 300, new Color32(251, 226, 196, 255), new Color32(122, 58, 10, 255), out statStreak);

        TextMeshProUGUI nextLabel;
        Image nextImg2 = Pill(card.transform, "next_button", "Next puzzle", 56, 816, 480, 84, Green, Color.white, 40, true, out nextLabel);
        Button resultNext = MakeButton(nextImg2);
        TextMeshProUGUI libLabel;
        Image libImg = Pill(card.transform, "library_button", "Library", 556, 816, 260, 84, Purple, Color.white, 36, true, out libLabel);
        Button resultLibrary = MakeButton(libImg);
        Image backOuter = LibraryUiKit.Box(card.transform, "back_button", Ink, true);
        LibraryUiKit.Place(backOuter.rectTransform, 836, 816, 228, 84);
        Image backInner = LibraryUiKit.Box(backOuter.transform, "inner", Cream, true);
        LibraryUiKit.Place(backInner.rectTransform, 4, 4, 220, 76);
        TextMeshProUGUI backLabel = Lab(backInner.transform, "Games", 36, Ink, false, TextAlignmentOptions.Center, 0, 0, 220, 76);
        LibraryUiKit.Fill(backLabel.rectTransform);
        Button resultBack = MakeButton(backOuter);

        // pause window
        Image pauseDim = LibraryUiKit.Box(overlays, "pause_panel", new Color(0.07f, 0.16f, 0.36f, 0.45f), false);
        LibraryUiKit.Fill(pauseDim.rectTransform);
        pauseDim.raycastTarget = true;
        Image pauseCard = LibraryUiKit.Box(pauseDim.transform, "card", Cream, true);
        LibraryUiKit.Place(pauseCard.rectTransform, 580, 120, 760, 840);
        TextMeshProUGUI pausedTitle;
        Pill(pauseCard.transform, "ribbon", "Paused", 140, -40, 480, 104, Navy, Color.white, 56, true, out pausedTitle);
        Image pauseIcon = LibraryUiKit.Icon(pauseCard.transform, "icon", LibraryUiKit.Circle, new Color32(251, 226, 196, 255));
        LibraryUiKit.Place(pauseIcon.rectTransform, 312, 108, 136, 136);
        Image pb1 = LibraryUiKit.Box(pauseIcon.transform, "bar_1", Orange, true);
        LibraryUiKit.Place(pb1.rectTransform, 38, 36, 24, 64);
        Image pb2 = LibraryUiKit.Box(pauseIcon.transform, "bar_2", Orange, true);
        LibraryUiKit.Place(pb2.rectTransform, 74, 36, 24, 64);
        Lab(pauseCard.transform, "Your timer is stopped", 40, InkSoft, false, TextAlignmentOptions.Center, 0, 262, 760, 56);
        Image timePill = LibraryUiKit.Box(pauseCard.transform, "time_pill", Sand, true);
        LibraryUiKit.Place(timePill.rectTransform, 110, 336, 540, 112);
        TextMeshProUGUI pauseTime = Lab(timePill.transform, "00:00", 60, Ink, true, TextAlignmentOptions.Center, 0, 0, 540, 112);
        LibraryUiKit.Fill(pauseTime.rectTransform);
        TextMeshProUGUI resumeLabel;
        Image resumeImg = Pill(pauseCard.transform, "resume_button", "Resume", 80, 480, 600, 112, Green, Color.white, 52, true, out resumeLabel);
        Button resumeButton = MakeButton(resumeImg);
        TextMeshProUGUI newLabel;
        Image newImg = Pill(pauseCard.transform, "new_puzzle_button", "New puzzle", 80, 616, 600, 92, Purple, Color.white, 40, true, out newLabel);
        Button newButton = MakeButton(newImg);
        Image pauseBackOuter = LibraryUiKit.Box(pauseCard.transform, "back_button", Ink, true);
        LibraryUiKit.Place(pauseBackOuter.rectTransform, 80, 728, 600, 80);
        Image pauseBackInner = LibraryUiKit.Box(pauseBackOuter.transform, "inner", Cream, true);
        LibraryUiKit.Place(pauseBackInner.rectTransform, 4, 4, 592, 72);
        TextMeshProUGUI pauseBackLabel = Lab(pauseBackInner.transform, "Back to games", 38, Ink, false, TextAlignmentOptions.Center, 0, 0, 592, 72);
        LibraryUiKit.Fill(pauseBackLabel.rectTransform);
        Button pauseBackButton = MakeButton(pauseBackOuter);

        // ---- point the existing grid generator and clue panel at the new parts
        var cellPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(CellPrefabPath).GetComponent<CrosswordCell>();
        var blackPrefab = (RectTransform)AssetDatabase.LoadAssetAtPath<GameObject>(BlackPrefabPath).transform;
        var rowPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(RowPrefabPath).GetComponent<CrosswordClueRow>();
        var style = AssetDatabase.LoadAssetAtPath<CrosswordTextStyle>(StylePath);

        var gso = new SerializedObject(generator);
        SetRef(gso, "gridParent", panelRect);
        SetRef(gso, "cellPrefab", cellPrefab);
        SetRef(gso, "blackCellPrefab", blackPrefab);
        SetRef(gso, "textStyle", style);
        gso.FindProperty("cellSize").floatValue = 80f;
        gso.FindProperty("spacing").floatValue = 4f;
        gso.FindProperty("fillFraction").floatValue = 0.98f;
        gso.FindProperty("gridOffset").vector2Value = Vector2.zero;
        gso.ApplyModifiedPropertiesWithoutUndo();

        var cso = new SerializedObject(cluePanel);
        SetRef(cso, "acrossListParent", acrossContent);
        SetRef(cso, "downListParent", downContent);
        SetRef(cso, "rowPrefab", rowPrefab);
        SetRef(cso, "textStyle", style);
        SetRef(cso, "activeClueNumberBadge", null);
        SetRef(cso, "activeClueText", null);
        cso.FindProperty("columnCount").intValue = 1;
        cso.FindProperty("rightMargin").floatValue = 8f;
        cso.FindProperty("rowSpacing").floatValue = 8f;
        cso.FindProperty("acrossOffset").vector2Value = new Vector2(0f, -4f);
        cso.FindProperty("downOffset").vector2Value = new Vector2(0f, -4f);
        cso.ApplyModifiedPropertiesWithoutUndo();

        // ---- controller
        RectTransform controllerRect = LibraryUiKit.NewRect("CrosswordController", canvas);
        LibraryUiKit.Fill(controllerRect);
        var controller = controllerRect.gameObject.AddComponent<CrosswordGameController>();
        var ko = new SerializedObject(controller);
        SetRef(ko, "input", input);
        SetRef(ko, "languageLabel", langLabel);
        SetRef(ko, "timerLabel", timerLabel);
        SetRef(ko, "pauseButton", pauseButton);
        SetRef(ko, "keypadToggle", keypadToggle);
        SetRef(ko, "keypadTrack", track);
        SetRef(ko, "keypadKnob", knob.rectTransform);
        SetRef(ko, "board", board);
        SetRef(ko, "toast", toast);
        SetRef(ko, "clueLabel", clueLabel);
        SetRef(ko, "clueText", clueText);
        SetRef(ko, "previousButton", prevButton);
        SetRef(ko, "nextButton", nextButton);
        SetRef(ko, "hintButton", hintButton);
        SetRef(ko, "hintLabel", hintLabel);
        SetRef(ko, "checkButton", checkButton);
        SetRef(ko, "revealButton", revealButton);
        SetRef(ko, "revealLabel", revealLabel);
        SetRef(ko, "listCard", listCard.rectTransform);
        SetRef(ko, "acrossTab", acrossTabButton);
        SetRef(ko, "acrossTabImage", acrossTabImg);
        SetRef(ko, "acrossTabLabel", acrossTabLabel);
        SetRef(ko, "downTab", downTabButton);
        SetRef(ko, "downTabImage", downTabImg);
        SetRef(ko, "downTabLabel", downTabLabel);
        SetRef(ko, "acrossScroll", acrossScroll);
        SetRef(ko, "downScroll", downScroll);
        SetRef(ko, "keypadRoot", keypad.gameObject);
        SetArray(ko, "keys", keys);
        SetRef(ko, "pausePanel", pauseDim.gameObject);
        SetRef(ko, "pauseTimeLabel", pauseTime);
        SetRef(ko, "resumeButton", resumeButton);
        SetRef(ko, "pauseNewButton", newButton);
        SetRef(ko, "pauseBackButton", pauseBackButton);
        SetRef(ko, "resultPanel", resultDim.gameObject);
        SetRef(ko, "resultCard", card.rectTransform);
        SetRef(ko, "resultTitle", resultTitle);
        SetRef(ko, "resultListContent", learnedContent);
        SetRef(ko, "resultRowTemplate", rowTemplate.gameObject);
        SetRef(ko, "statTime", statTime);
        SetRef(ko, "statHints", statHints);
        SetRef(ko, "statStreak", statStreak);
        SetRef(ko, "resultNextButton", resultNext);
        SetRef(ko, "resultLibraryButton", resultLibrary);
        SetRef(ko, "resultBackButton", resultBack);
        SetRef(ko, "confettiRoot", confetti);
        ko.ApplyModifiedPropertiesWithoutUndo();

        // panels start hidden in the saved scene; only the Across list is shown
        resultDim.gameObject.SetActive(false);
        pauseDim.gameObject.SetActive(false);
        downScroll.gameObject.SetActive(false);

        // final order: overlays on top, controller last
        overlays.SetAsLastSibling();
        controllerRect.SetAsLastSibling();

        // ---- build the puzzle in edit mode so the design is visible before Play
        var isoLang = new SerializedObject(input);
        var puzzleLanguage = (PuzzleLanguage)isoLang.FindProperty("language").enumValueIndex;
        int level = isoLang.FindProperty("difficultyLevel").intValue;
        CrosswordPuzzle puzzle = CrosswordPuzzleLibrary.Get(puzzleLanguage, level);
        input.Initialize(puzzle);
        controller.RefreshPreview();

        EditorUtility.SetDirty(canvasGo);
        return "built (" + language + " level " + level + ", " + puzzle.words.Count + " words, " + keys.Count + " keys)";
    }
}
