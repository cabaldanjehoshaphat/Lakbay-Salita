using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// CrosswordCategoryBuilder
/// Editor tool that sets up the Crossword categories as real saved scenes, in the same look as the menus and the redesigned Crossword:
///  - it archives the 30 current level scenes (the 2nd design) in Assets/Main/Scenes/3 Crossword/Archive/2nd design, renamed with
///    "(2nd design 2026-10-08)", so they can be brought back if needed (the 1st design archive stays where it is),
///  - "Crossword - Start": back button, Crossword title chip, language chip, a "Random puzzle" button and seven category cards. Each card has a solved
///    tag ("1 of 3 solved"), the size navigator (arrow stepper "Medium / 8 words") and a Play button; an info card explains the screen.
///    Driven by CrosswordStartController,
///  - "Crossword - Play": one play scene for every category puzzle. It is a copy of the redesigned Cebuano level 10 scene (taken before it is
///    archived) whose puzzle comes from code (CrosswordCategoryBootstrap reads CrosswordSession) and which has a category chip (colour dot,
///    name, "Medium · 8 words") in the top bar,
///  - the Crossword card on the language hub (3 Language_Selection_Menu): its "10 levels" tag becomes the progress tag ("N of 21 solved",
///    CrosswordHubTag).
/// Both new scenes are saved in Assets/Main/Scenes/3 Crossword and added to Build Settings. Menu: Tools > Crossword > "Archive current
/// crossword and build category scenes". Running it again rebuilds the two new scenes; it only archives scenes that are still in the old place.
/// </summary>
public static class CrosswordCategoryBuilder
{
    private const string SceneFolder = "Assets/Main/Scenes/3 Crossword";
    private const string ArchiveFolder = SceneFolder + "/Archive/2nd design";
    private const string StartPath = SceneFolder + "/Crossword - Start.unity";
    private const string PlayPath = SceneFolder + "/Crossword - Play.unity";
    private const string PlaySourcePath = SceneFolder + "/1 - Puzzle - Cebuano/puzzle-crossword-generator 10 Cebuano.unity";
    private const string ArchiveTag = " (2nd design 2026-10-08)";
    private const string HubPath = "Assets/Main/Scenes/1 Menu/3 Language_Selection_Menu.unity";
    private const string MaterialPath = "Assets/Main/Sprites/1 Menu/Main Menu/LiberationSans SDF Colorable.mat";
    private const string SpriteFolder = "Assets/Main/Sprites/1 Menu/Main Menu/";

    private static readonly Color Cream = new Color32(253, 248, 238, 255);
    private static readonly Color Ink = new Color32(59, 42, 26, 255);
    private static readonly Color InkSoft = new Color32(107, 82, 55, 255);
    private static readonly Color Blue = new Color32(55, 138, 221, 255);
    private static readonly Color DarkBlue = new Color32(27, 58, 140, 255);
    private static readonly Color Sand = new Color32(239, 231, 212, 255);

    private static Material _mat;

    // ------------------------------------------------------------------ menu entry

    [MenuItem("Tools/Crossword/Archive current crossword and build category scenes")]
    public static void BuildAllMenu()
    {
        Debug.Log(BuildAll());
    }

    /// <summary>Copies the play scene, archives the 30 level scenes, builds the start scene, finishes the play scene and updates the hub tag.</summary>
    public static string BuildAll()
    {
        // 1. the play scene is a copy of a level scene, so it is copied before the level scenes move
        string copyResult = CopyPlaySource();

        // 2. archive the current (2nd design) level scenes
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        string archive = ArchiveCurrentScenes();

        // 3. new scenes
        string start = BuildStartScene();
        string play = copyResult == null ? ConfigurePlayScene() : copyResult;
        AddToBuildSettings(StartPath);
        AddToBuildSettings(PlayPath);

        // 4. hub tag
        string hub = UpdateHubTag();
        AssetDatabase.SaveAssets();
        return "Archive: " + archive + " | Start: " + start + " | Play: " + play + " | Hub: " + hub;
    }

    private static void AddToBuildSettings(string path)
    {
        var scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        foreach (EditorBuildSettingsScene s in scenes)
        {
            if (s.path == path)
            {
                s.enabled = true;
                EditorBuildSettings.scenes = scenes.ToArray();
                return;
            }
        }
        scenes.Add(new EditorBuildSettingsScene(path, true));
        EditorBuildSettings.scenes = scenes.ToArray();
    }

    // ------------------------------------------------------------------ archive

    private static void EnsureFolder(string parent, string name)
    {
        if (!AssetDatabase.IsValidFolder(parent + "/" + name))
        {
            AssetDatabase.CreateFolder(parent, name);
        }
    }

    private static string CopyPlaySource()
    {
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(PlaySourcePath) == null)
        {
            // already archived by an earlier run: copy it from the archive instead
            string archived = ArchiveFolder + "/1 - Puzzle - Cebuano/puzzle-crossword-generator 10 Cebuano" + ArchiveTag + ".unity";
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(archived) == null)
            {
                return "SKIPPED (the level 10 Cebuano scene to copy is missing)";
            }
            return CopyPlay(archived);
        }
        return CopyPlay(PlaySourcePath);
    }

    private static string CopyPlay(string source)
    {
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(PlayPath) != null)
        {
            AssetDatabase.DeleteAsset(PlayPath);
        }
        if (!AssetDatabase.CopyAsset(source, PlayPath))
        {
            return "FAILED to copy " + source;
        }
        AssetDatabase.ImportAsset(PlayPath);
        return null;
    }

    private static string ArchiveCurrentScenes()
    {
        EnsureFolder(SceneFolder + "/Archive", "2nd design");
        string[] folders = { "1 - Puzzle - Cebuano", "2 - Puzzle - Ilonggo", "3 - Puzzle - Tagalog" };
        string[] langs = { "Cebuano", "Ilonggo", "Tagalog" };
        int moved = 0, skipped = 0;
        for (int l = 0; l < 3; l++)
        {
            EnsureFolder(ArchiveFolder, folders[l]);
            for (int level = 1; level <= 10; level++)
            {
                string name = "puzzle-crossword-generator " + level + " " + langs[l];
                string from = SceneFolder + "/" + folders[l] + "/" + name + ".unity";
                string to = ArchiveFolder + "/" + folders[l] + "/" + name + ArchiveTag + ".unity";
                if (AssetDatabase.LoadAssetAtPath<SceneAsset>(from) == null)
                {
                    skipped++;
                    continue;
                }
                string error = AssetDatabase.MoveAsset(from, to);
                if (!string.IsNullOrEmpty(error))
                {
                    Debug.LogWarning("CrosswordCategoryBuilder: could not archive " + from + ": " + error);
                    skipped++;
                    continue;
                }
                moved++;
            }
        }
        AssetDatabase.SaveAssets();
        return moved + " scenes moved to " + ArchiveFolder + (skipped > 0 ? " (" + skipped + " skipped)" : string.Empty);
    }

    // ------------------------------------------------------------------ small helpers

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
        colors.disabledColor = new Color(1f, 1f, 1f, 0.4f);
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
            Debug.LogWarning("CrosswordCategoryBuilder: missing field " + name);
            return;
        }
        p.objectReferenceValue = value;
    }

    private static void SetArray<T>(SerializedObject so, string name, IList<T> items) where T : Object
    {
        SerializedProperty p = so.FindProperty(name);
        if (p == null)
        {
            Debug.LogWarning("CrosswordCategoryBuilder: missing array " + name);
            return;
        }
        p.arraySize = items.Count;
        for (int i = 0; i < items.Count; i++)
        {
            p.GetArrayElementAtIndex(i).objectReferenceValue = items[i];
        }
    }

    /// <summary>A rounded square button with an arrow (the triangle sprite) inside; leftArrow turns the arrow around.</summary>
    private static Button ArrowButton(Transform parent, string name, float x, float y, float size, bool leftArrow)
    {
        Image img = LibraryUiKit.Box(parent, name, Cream, true);
        LibraryUiKit.Place(img.rectTransform, x, y, size, size);
        Image arrow = LibraryUiKit.Icon(img.transform, "arrow", Spr("main_menu_play_triangle.png"), Ink);
        float aw = size * 0.4f, ah = size * 0.46f;
        LibraryUiKit.Place(arrow.rectTransform, (size - aw) / 2f, (size - ah) / 2f, aw, ah);
        arrow.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        arrow.rectTransform.anchoredPosition = new Vector2(size / 2f, -size / 2f);
        if (leftArrow)
        {
            arrow.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 180f);
        }
        return MakeButton(img);
    }

    /// <summary>Creates an empty scene with a camera, a 1920x1080 canvas (screen-space camera), an EventSystem and the sky base colour.</summary>
    private static Transform NewScene(out Image skyBase)
    {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        var camGo = new GameObject("Main Camera");
        camGo.tag = "MainCamera";
        camGo.transform.position = new Vector3(0f, 0f, -10f);
        var cam = camGo.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 5f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color32(95, 180, 236, 255);
        camGo.AddComponent<AudioListener>();

        var canvasGo = new GameObject("Canvas", typeof(RectTransform));
        canvasGo.layer = 5;
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = cam;
        canvas.planeDistance = 100f;
        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        canvasGo.AddComponent<GraphicRaycaster>();

        new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));

        var bgGo = new GameObject("sky_base", typeof(RectTransform));
        bgGo.layer = 5;
        bgGo.transform.SetParent(canvasGo.transform, false);
        skyBase = bgGo.AddComponent<Image>();
        skyBase.color = new Color32(95, 180, 236, 255);
        skyBase.raycastTarget = false;
        LibraryUiKit.Fill(skyBase.rectTransform);
        return canvasGo.transform;
    }

    /// <summary>Sky layers (gradient, stars and moon, sun, clouds) plus the MainMenuTheme that fades them by time of day.</summary>
    private static void BuildSky(Transform canvas, RectTransform ui, Image skyBase)
    {
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

        RectTransform themeRect = LibraryUiKit.NewRect("CrosswordStartTheme", canvas);
        LibraryUiKit.Fill(themeRect);
        var theme = themeRect.gameObject.AddComponent<MainMenuTheme>();
        var themeSo = new SerializedObject(theme);
        SetRef(themeSo, "skyBase", skyBase);
        SetRef(themeSo, "skyTopOverlay", skyTop);
        SetRef(themeSo, "clouds", cloudG);
        SetRef(themeSo, "sun", sunG);
        SetRef(themeSo, "night", nightG);
        themeSo.ApplyModifiedPropertiesWithoutUndo();
        theme.LoadDefaultThemes();
        theme.PreviewPeriod(DayPeriod.Noon);
    }

    // ------------------------------------------------------------------ start scene

    private static string BuildStartScene()
    {
        Image bg;
        Transform canvas = NewScene(out bg);
        RectTransform ui = LibraryUiKit.NewRect("CrosswordStartUI", canvas);
        LibraryUiKit.Fill(ui);
        BuildSky(canvas, ui, bg);

        // ---- top bar
        Image backRing = LibraryUiKit.Icon(ui, "back_button", LibraryUiKit.Circle, Cream);
        LibraryUiKit.Place(backRing.rectTransform, 48, 40, 112, 112);
        Image backArrow = LibraryUiKit.Icon(backRing.transform, "arrow", Spr("main_menu_play_triangle.png"), Ink);
        LibraryUiKit.Place(backArrow.rectTransform, 36, 30, 40, 52);
        backArrow.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        backArrow.rectTransform.anchoredPosition = new Vector2(36f + 20f, -(30f + 26f));
        backArrow.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 180f);
        Button backButton = MakeButton(backRing);

        Image titleChip = LibraryUiKit.Box(ui, "title_chip", Cream, true);
        LibraryUiKit.Place(titleChip.rectTransform, 190, 52, 380, 88);
        Image gridTile = LibraryUiKit.Box(titleChip.transform, "grid_tile", Blue, true);
        LibraryUiKit.Place(gridTile.rectTransform, 16, 16, 56, 56);
        foreach (float[] sq in new[] { new[] { 10f, 10f }, new[] { 31f, 10f }, new[] { 10f, 31f }, new[] { 31f, 31f } })
        {
            Image s = LibraryUiKit.Box(gridTile.transform, "square", Color.white, true);
            LibraryUiKit.Place(s.rectTransform, sq[0], sq[1], 15, 15);
        }
        Lab(titleChip.transform, "Crossword", 48, Ink, true, TextAlignmentOptions.MidlineLeft, 90, 0, 280, 88);

        TextMeshProUGUI langLabel;
        Pill(ui, "language_chip", "Cebuano", 590, 60, 224, 64, Cream, Ink, 36, false, out langLabel);

        TextMeshProUGUI randomLabel;
        Image randomImg = Pill(ui, "random_button", "Random puzzle", 1520, 52, 360, 88, Blue, Color.white, 40, true, out randomLabel);
        Button randomButton = MakeButton(randomImg);

        // ---- category cards: 4 on the first row, 3 + the info card on the second
        string[] fallbackNames = { "Animals", "Food and drink", "Body", "Family and people", "Home and things", "Nature and weather", "Numbers" };
        var names = new List<TMP_Text>();
        var solvedLabels = new List<TMP_Text>();
        var solvedPills = new List<Image>();
        var sizeLabels = new List<TMP_Text>();
        var prevButtons = new List<Button>();
        var nextButtons = new List<Button>();
        var playButtons = new List<Button>();
        for (int i = 0; i < 7; i++)
        {
            float x = 60 + (i % 4) * 460;
            float y = 216 + (i / 4) * 420;
            CrosswordCategoryDef def = CrosswordCategoryData.Category(0, i);
            Image card = LibraryUiKit.Box(ui, "category_" + (i + 1), Cream, true);
            LibraryUiKit.Place(card.rectTransform, x, y, 420, 380);

            Image dot = LibraryUiKit.Icon(card.transform, "dot", LibraryUiKit.Circle, WordleData.CategoryColor(i));
            LibraryUiKit.Place(dot.rectTransform, 28, 24, 52, 52);
            TextMeshProUGUI nameLabel = Lab(card.transform, def != null ? def.name : fallbackNames[i], 40, Ink, true,
                TextAlignmentOptions.MidlineLeft, 92, 16, 300, 68);
            nameLabel.enableWordWrapping = true;
            nameLabel.enableAutoSizing = true;
            nameLabel.fontSizeMin = 26f;
            nameLabel.fontSizeMax = 40f;
            nameLabel.overflowMode = TextOverflowModes.Overflow;
            names.Add(nameLabel);

            TextMeshProUGUI solvedLabel;
            Image solvedPill = Pill(card.transform, "solved_tag", "0 of 3 solved", 28, 100, 200, 44, Sand, Ink, 24, true, out solvedLabel);
            solvedLabels.Add(solvedLabel);
            solvedPills.Add(solvedPill);

            Image stepper = LibraryUiKit.Box(card.transform, "size_navigator", Sand, true);
            LibraryUiKit.Place(stepper.rectTransform, 28, 160, 364, 84);
            prevButtons.Add(ArrowButton(stepper.transform, "previous_size", 6, 6, 72, true));
            TextMeshProUGUI sizeLabel = Lab(stepper.transform, "Small", 40, Ink, true, TextAlignmentOptions.Center, 84, 0, 196, 84);
            sizeLabel.enableWordWrapping = false;
            sizeLabel.overflowMode = TextOverflowModes.Overflow;
            sizeLabels.Add(sizeLabel);
            nextButtons.Add(ArrowButton(stepper.transform, "next_size", 286, 6, 72, false));

            TextMeshProUGUI playLabel;
            Image playImg = Pill(card.transform, "play_button", "Play", 28, 262, 364, 92, Blue, Color.white, 46, true, out playLabel);
            playButtons.Add(MakeButton(playImg));
        }

        Image info = LibraryUiKit.Box(ui, "info_card", DarkBlue, true);
        LibraryUiKit.Place(info.rectTransform, 60 + 3 * 460, 216 + 420, 420, 380);
        Lab(info.transform, "Pick a topic", 52, new Color32(232, 185, 35, 255), true, TextAlignmentOptions.TopLeft, 28, 36, 364, 70);
        TextMeshProUGUI infoText = Lab(info.transform, "Use the arrows to choose a size, then press Play. Small has 5 words, Medium 8 and Large 11. Every answer fits the topic.", 30,
            Color.white, false, TextAlignmentOptions.TopLeft, 28, 118, 364, 220);
        infoText.enableWordWrapping = true;
        infoText.overflowMode = TextOverflowModes.Overflow;

        // ---- controller
        RectTransform controllerRect = LibraryUiKit.NewRect("CrosswordStartController", canvas);
        LibraryUiKit.Fill(controllerRect);
        var controller = controllerRect.gameObject.AddComponent<CrosswordStartController>();
        var so = new SerializedObject(controller);
        SetRef(so, "languageLabel", langLabel);
        SetRef(so, "backButton", backButton);
        SetRef(so, "randomButton", randomButton);
        SetArray(so, "categoryNames", names);
        SetArray(so, "solvedLabels", solvedLabels);
        SetArray(so, "solvedPills", solvedPills);
        SetArray(so, "sizeLabels", sizeLabels);
        SetArray(so, "previousButtons", prevButtons);
        SetArray(so, "nextButtons", nextButtons);
        SetArray(so, "playButtons", playButtons);
        so.ApplyModifiedPropertiesWithoutUndo();
        controllerRect.SetAsLastSibling();

        CrosswordSession.Language = 0;
        controller.Refresh();

        Scene scene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.SaveScene(scene, StartPath);
        return "saved " + StartPath + " (" + playButtons.Count + " category cards)";
    }

    // ------------------------------------------------------------------ play scene

    private static string ConfigurePlayScene()
    {
        EditorSceneManager.OpenScene(PlayPath, OpenSceneMode.Single);

        var canvasGo = GameObject.Find("Canvas");
        var input = Object.FindObjectOfType<CrosswordInputController>();
        var controller = Object.FindObjectOfType<CrosswordGameController>();
        if (canvasGo == null || input == null || controller == null)
        {
            return "SKIPPED (the copied scene is missing the Canvas, the input controller or the game controller)";
        }
        Transform ui = canvasGo.transform.Find("CrosswordUI");
        if (ui == null)
        {
            return "SKIPPED (no CrosswordUI in the copied scene)";
        }

        // category chip in the top bar (between the timer and the Keypad switch), below the toast so the toast can cover it
        Transform oldChip = ui.Find("category_chip");
        if (oldChip != null)
        {
            Object.DestroyImmediate(oldChip.gameObject);
        }
        Image chip = LibraryUiKit.Box(ui, "category_chip", Cream, true);
        LibraryUiKit.Place(chip.rectTransform, 800, 48, 440, 88);
        Image dot = LibraryUiKit.Icon(chip.transform, "dot", LibraryUiKit.Circle, WordleData.CategoryColor(0));
        LibraryUiKit.Place(dot.rectTransform, 20, 30, 28, 28);
        TextMeshProUGUI nameLabel = Lab(chip.transform, "Animals", 32, Ink, true, TextAlignmentOptions.MidlineLeft, 62, 6, 366, 44);
        nameLabel.enableAutoSizing = true;
        nameLabel.fontSizeMin = 20f;
        nameLabel.fontSizeMax = 32f;
        TextMeshProUGUI sizeLabel = Lab(chip.transform, "Small · 5 words", 26, InkSoft, false, TextAlignmentOptions.MidlineLeft, 62, 48, 366, 34);
        Transform toast = ui.Find("toast");
        if (toast != null)
        {
            chip.rectTransform.SetSiblingIndex(toast.GetSiblingIndex());
        }

        // bootstrap on the controller object (runs first, execution order -200)
        var bootstrap = controller.gameObject.GetComponent<CrosswordCategoryBootstrap>();
        if (bootstrap == null)
        {
            bootstrap = controller.gameObject.AddComponent<CrosswordCategoryBootstrap>();
        }
        var so = new SerializedObject(bootstrap);
        SetRef(so, "input", input);
        SetRef(so, "controller", controller);
        SetRef(so, "categoryDot", dot);
        SetRef(so, "categoryNameLabel", nameLabel);
        SetRef(so, "categorySizeLabel", sizeLabel);
        so.ApplyModifiedPropertiesWithoutUndo();

        EditorUtility.SetDirty(canvasGo);
        Scene scene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, PlayPath);
        return "saved " + PlayPath + " (copy of the Cebuano level 10 scene + category chip + bootstrap)";
    }

    // ------------------------------------------------------------------ hub tag

    private static string UpdateHubTag()
    {
        EditorSceneManager.OpenScene(HubPath, OpenSceneMode.Single);
        var card = GameObject.Find("crossword_card");
        if (card == null)
        {
            return "SKIPPED (no crossword_card in the hub scene)";
        }
        Transform pill = card.transform.Find("Pill");
        if (pill == null)
        {
            return "SKIPPED (no tag on the Crossword card)";
        }

        // the tag is wider than the old "10 levels" tag so "Cebuano · 4 of 21" fits; the saved text shows the design in the editor
        // (CrosswordHubTag rewrites it while playing)
        var rt = (RectTransform)pill;
        rt.anchoredPosition = new Vector2(670f, -60f);
        rt.sizeDelta = new Vector2(300f, 58f);
        TMP_Text label = pill.GetComponentInChildren<TMP_Text>();
        label.text = "Cebuano · 0 of 21";
        label.enableAutoSizing = true;
        label.fontSizeMin = 20f;
        label.fontSizeMax = 28f;

        var tag = pill.GetComponent<CrosswordHubTag>();
        if (tag == null)
        {
            tag = pill.gameObject.AddComponent<CrosswordHubTag>();
        }
        var so = new SerializedObject(tag);
        SetRef(so, "languageSelector", Object.FindObjectOfType<LanguageSelector>());
        SetRef(so, "label", label);
        SetRef(so, "background", pill.GetComponent<Image>());
        so.ApplyModifiedPropertiesWithoutUndo();

        Scene scene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        return "Crossword tag set to the progress tag (" + rt.anchoredPosition + ", " + rt.sizeDelta + ")";
    }
}
