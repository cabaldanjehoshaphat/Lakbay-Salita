using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// LibraryUiKit
/// Helper set used by LibraryController / LibraryNotebook to build the Library screen from code: the
/// shared color palette, sprites (rounded card, circle, star) and one-line factory methods for rects,
/// boxes, text labels and buttons. Everything is placed with top-left anchoring: Place(rect, x, y,
/// width, height) where y grows downward from the parent's top edge, in canvas pixels (reference
/// 1920x1080).
///
/// The screen is built once in Edit mode and saved in the scene, so it is visible before Play. To let
/// Play re-attach to those saved objects instead of creating new ones, BeginReuse()/EndReuse() switch
/// the factories into "reuse" mode: the k-th NewRect call under a parent returns that parent's k-th
/// existing child with that name (children that Unity adds itself, such as an input field's "Caret",
/// are skipped), and components are fetched with GetOrAdd. In the editor the
/// sprites are real PNG assets in Assets/Main/Sprites/5 Library (created automatically if missing) so
/// the saved scene can reference them; in a player build they are generated at runtime.
/// </summary>
public static class LibraryUiKit
{
    public static readonly Color Ink = new Color32(59, 42, 26, 255);
    public static readonly Color InkSoft = new Color32(107, 82, 55, 255);
    public static readonly Color Paper = new Color32(255, 250, 240, 255);
    public static readonly Color Line = new Color32(234, 223, 201, 255);
    public static readonly Color Accent = new Color32(245, 166, 35, 255);
    public static readonly Color Red = new Color32(163, 45, 45, 255);
    public static readonly Color StarOff = new Color32(185, 169, 138, 255);
    public static readonly Color CardTint = new Color(1f, 0.98f, 0.94f, 0.88f);
    public static readonly Color[] LangColors =
    {
        new Color32(59, 109, 17, 255),
        new Color32(24, 95, 165, 255),
        new Color32(153, 53, 86, 255)
    };

    private static Sprite _rounded;
    private static Sprite _circle;
    private static Sprite _star;

    private static bool _reuse;
    private static readonly Dictionary<Transform, int> Cursor = new Dictionary<Transform, int>();

    public static void BeginReuse()
    {
        _reuse = true;
        Cursor.Clear();
    }

    public static void EndReuse()
    {
        _reuse = false;
        Cursor.Clear();
    }

    public static T GetOrAdd<T>(GameObject go) where T : Component
    {
        T c = go.GetComponent<T>();
        return c != null ? c : go.AddComponent<T>();
    }

    /// <summary>Destroys immediately in Edit mode and at the end of the frame while playing.</summary>
    public static void DestroyObject(GameObject go)
    {
        if (Application.isPlaying) Object.Destroy(go);
        else Object.DestroyImmediate(go);
    }

    // ------------------------------------------------------------------ sprites

    public static Sprite Rounded
    {
        get
        {
            if (_rounded == null) _rounded = MakeSprite("library_rounded.png", RoundedTexture(64, 24), 26);
            return _rounded;
        }
    }

    public static Sprite Circle
    {
        get
        {
            if (_circle == null) _circle = MakeSprite("library_circle.png", CircleTexture(64), 0);
            return _circle;
        }
    }

    public static Sprite Star
    {
        get
        {
            if (_star == null) _star = MakeSprite("library_star.png", StarTexture(64), 0);
            return _star;
        }
    }

    private static Sprite MakeSprite(string fileName, Texture2D tex, int border)
    {
#if UNITY_EDITOR
        string path = "Assets/Main/Sprites/5 Library/" + fileName;
        var existing = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (existing != null)
        {
            Object.DestroyImmediate(tex);
            return existing;
        }
        string full = System.IO.Path.Combine(System.IO.Directory.GetParent(Application.dataPath).FullName, path);
        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(full));
        System.IO.File.WriteAllBytes(full, tex.EncodeToPNG());
        Object.DestroyImmediate(tex);
        UnityEditor.AssetDatabase.ImportAsset(path, UnityEditor.ImportAssetOptions.ForceSynchronousImport);
        var importer = UnityEditor.AssetImporter.GetAtPath(path) as UnityEditor.TextureImporter;
        if (importer != null)
        {
            importer.textureType = UnityEditor.TextureImporterType.Sprite;
            importer.spriteImportMode = UnityEditor.SpriteImportMode.Single;
            importer.spritePixelsPerUnit = 100f;
            importer.spriteBorder = new Vector4(border, border, border, border);
            importer.mipmapEnabled = false;
            importer.filterMode = FilterMode.Bilinear;
            importer.alphaIsTransparency = true;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.textureCompression = UnityEditor.TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
        }
        return UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(path);
#else
        float b = border;
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, new Vector4(b, b, b, b));
#endif
    }

    private static Texture2D NewTexture(int size)
    {
        return new Texture2D(size, size, TextureFormat.RGBA32, false) { filterMode = FilterMode.Bilinear, wrapMode = TextureWrapMode.Clamp };
    }

    private static Texture2D RoundedTexture(int size, int radius)
    {
        Texture2D tex = NewTexture(size);
        var px = new Color32[size * size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float cx = Mathf.Clamp(x + 0.5f, radius, size - radius);
                float cy = Mathf.Clamp(y + 0.5f, radius, size - radius);
                float d = Mathf.Sqrt((x + 0.5f - cx) * (x + 0.5f - cx) + (y + 0.5f - cy) * (y + 0.5f - cy));
                px[y * size + x] = new Color32(255, 255, 255, (byte)(Mathf.Clamp01(radius - d + 0.5f) * 255f));
            }
        }
        tex.SetPixels32(px);
        tex.Apply();
        return tex;
    }

    private static Texture2D CircleTexture(int size)
    {
        Texture2D tex = NewTexture(size);
        var px = new Color32[size * size];
        float r = size / 2f;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float d = Mathf.Sqrt((x + 0.5f - r) * (x + 0.5f - r) + (y + 0.5f - r) * (y + 0.5f - r));
                px[y * size + x] = new Color32(255, 255, 255, (byte)(Mathf.Clamp01(r - d + 0.5f) * 255f));
            }
        }
        tex.SetPixels32(px);
        tex.Apply();
        return tex;
    }

    private static Texture2D StarTexture(int size)
    {
        Texture2D tex = NewTexture(size);
        var poly = new Vector2[10];
        float c = size / 2f;
        for (int i = 0; i < 10; i++)
        {
            float ang = Mathf.PI / 2f + i * Mathf.PI / 5f;
            float rad = (i % 2 == 0 ? 0.48f : 0.20f) * size;
            poly[i] = new Vector2(c + Mathf.Cos(ang) * rad, c + Mathf.Sin(ang) * rad);
        }
        var px = new Color32[size * size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                int hit = 0;
                for (int sy = 0; sy < 3; sy++)
                {
                    for (int sx = 0; sx < 3; sx++)
                    {
                        if (Inside(poly, new Vector2(x + (sx + 0.5f) / 3f, y + (sy + 0.5f) / 3f))) hit++;
                    }
                }
                px[y * size + x] = new Color32(255, 255, 255, (byte)(hit * 255 / 9));
            }
        }
        tex.SetPixels32(px);
        tex.Apply();
        return tex;
    }

    private static bool Inside(Vector2[] poly, Vector2 p)
    {
        bool inside = false;
        for (int i = 0, j = poly.Length - 1; i < poly.Length; j = i++)
        {
            if ((poly[i].y > p.y) != (poly[j].y > p.y) &&
                p.x < (poly[j].x - poly[i].x) * (p.y - poly[i].y) / (poly[j].y - poly[i].y) + poly[i].x)
            {
                inside = !inside;
            }
        }
        return inside;
    }

    // ---------------------------------------------------------------- factories

    public static RectTransform NewRect(string name, Transform parent)
    {
        if (_reuse)
        {
            int c;
            Cursor.TryGetValue(parent, out c);
            for (int i = c; i < parent.childCount; i++)
            {
                Transform t = parent.GetChild(i);
                if (t.name == name && t is RectTransform)
                {
                    Cursor[parent] = i + 1;
                    return (RectTransform)t;
                }
            }
        }
        var go = new GameObject(name, typeof(RectTransform));
        go.layer = 5;
        var r = (RectTransform)go.transform;
        r.SetParent(parent, false);
        r.anchorMin = r.anchorMax = new Vector2(0f, 1f);
        r.pivot = new Vector2(0f, 1f);
        return r;
    }

    public static void Place(RectTransform r, float x, float y, float w, float h)
    {
        r.anchorMin = r.anchorMax = new Vector2(0f, 1f);
        r.pivot = new Vector2(0f, 1f);
        r.anchoredPosition = new Vector2(x, -y);
        r.sizeDelta = new Vector2(w, h);
    }

    public static void Fill(RectTransform r)
    {
        r.anchorMin = Vector2.zero;
        r.anchorMax = Vector2.one;
        r.pivot = new Vector2(0.5f, 0.5f);
        r.offsetMin = Vector2.zero;
        r.offsetMax = Vector2.zero;
    }

    public static Image Box(Transform parent, string name, Color color, bool rounded)
    {
        RectTransform r = NewRect(name, parent);
        var img = GetOrAdd<Image>(r.gameObject);
        img.color = color;
        img.raycastTarget = false;
        if (rounded)
        {
            img.sprite = Rounded;
            img.type = Image.Type.Sliced;
        }
        return img;
    }

    public static Image Icon(Transform parent, string name, Sprite sprite, Color color)
    {
        RectTransform r = NewRect(name, parent);
        var img = GetOrAdd<Image>(r.gameObject);
        img.sprite = sprite;
        img.color = color;
        img.raycastTarget = false;
        return img;
    }

    public static TextMeshProUGUI Label(Transform parent, string text, float size, Color color,
        FontStyles style = FontStyles.Normal, TextAlignmentOptions align = TextAlignmentOptions.TopLeft, bool wrap = false)
    {
        RectTransform r = NewRect("Text", parent);
        var t = GetOrAdd<TextMeshProUGUI>(r.gameObject);
        t.text = text;
        t.fontSize = size;
        t.color = color;
        t.fontStyle = style;
        t.alignment = align;
        t.enableWordWrapping = wrap;
        t.overflowMode = TextOverflowModes.Ellipsis;
        t.raycastTarget = false;
        return t;
    }

    /// <summary>Makes the image clickable (it must be the topmost raycast target at that spot).</summary>
    public static Button MakeButton(Image target, UnityAction onClick)
    {
        target.raycastTarget = true;
        var b = GetOrAdd<Button>(target.gameObject);
        b.targetGraphic = target;
        b.transition = Selectable.Transition.None;
        if (onClick != null)
        {
            b.onClick.AddListener(onClick);
        }
        return b;
    }

    /// <summary>A rounded pill with centered text; returns the pill image (add a button to it if needed).</summary>
    public static Image Pill(Transform parent, string text, float x, float y, float w, float h, Color bg, Color fg, float fontSize,
        FontStyles style = FontStyles.Normal)
    {
        Image img = Box(parent, "Pill", bg, true);
        Place(img.rectTransform, x, y, w, h);
        TextMeshProUGUI t = Label(img.transform, text, fontSize, fg, style, TextAlignmentOptions.Center);
        Fill(t.rectTransform);
        return img;
    }

    /// <summary>A viewport + content pair inside a ScrollRect, content anchored to the top and stretched horizontally.</summary>
    public static ScrollRect MakeScroll(Transform parent, string name, float x, float y, float w, float h, out RectTransform content)
    {
        RectTransform root = NewRect(name, parent);
        Place(root, x, y, w, h);
        var scroll = GetOrAdd<ScrollRect>(root.gameObject);

        RectTransform viewport = NewRect("Viewport", root);
        Fill(viewport);
        var vImg = GetOrAdd<Image>(viewport.gameObject);
        vImg.color = new Color(1f, 1f, 1f, 0f);
        GetOrAdd<RectMask2D>(viewport.gameObject);

        content = NewRect("Content", viewport);
        content.anchorMin = new Vector2(0f, 1f);
        content.anchorMax = new Vector2(1f, 1f);
        content.pivot = new Vector2(0.5f, 1f);
        content.anchoredPosition = Vector2.zero;
        content.sizeDelta = new Vector2(0f, 100f);

        scroll.viewport = viewport;
        scroll.content = content;
        scroll.horizontal = false;
        scroll.vertical = true;
        scroll.movementType = ScrollRect.MovementType.Clamped;
        scroll.scrollSensitivity = 60f;
        scroll.inertia = true;
        scroll.decelerationRate = 0.12f;
        return scroll;
    }
}
