using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ImageContainer
/// Reusable ScriptableObject data asset holding letter-to-sprite pairs (e.g. 'A' =>
/// "Alphabet A image.png") for the four common visual states an image/tile can be in:
/// Highlighted, Normal, Plain, and Wrong. Mirrors the same one-script, one-list-per-state
/// pattern as AlphabetData
/// (Assets/Sub/Word_Search/Scripts/ScriptableObjects/AlphabetData.cs), but generic —
/// not tied to the Word Search feature — so it can back any game's letter/image states
/// (Wordle, Crossword, Word Search, etc). Create one via Assets > Create > Data >
/// Image Container, fill in each state's letters/sprites in the Inspector (via the
/// matching ImageContainerDrawer custom editor), then look sprites up at runtime with
/// GetSprite(letter, state).
/// </summary>
[CreateAssetMenu(fileName = "ImageContainer", menuName = "Data/Image Container")]
public class ImageContainer : ScriptableObject
{
    public enum ImageState
    {
        Highlighted,
        Normal,
        Plain,
        Wrong
    }

    [System.Serializable]
    public class ImageEntry
    {
        public char letter;
        public Sprite image;
    }

    public List<ImageEntry> highlighted = new List<ImageEntry>();
    public List<ImageEntry> normal = new List<ImageEntry>();
    public List<ImageEntry> plain = new List<ImageEntry>();
    public List<ImageEntry> wrong = new List<ImageEntry>();

    /// <summary>Looks up the sprite for a given letter in the given state. Returns null if not found.</summary>
    public Sprite GetSprite(char letter, ImageState state)
    {
        List<ImageEntry> list = GetList(state);
        ImageEntry entry = list?.Find(e => e.letter == letter);
        return entry != null ? entry.image : null;
    }

    private List<ImageEntry> GetList(ImageState state)
    {
        switch (state)
        {
            case ImageState.Highlighted: return highlighted;
            case ImageState.Normal: return normal;
            case ImageState.Plain: return plain;
            case ImageState.Wrong: return wrong;
            default: return null;
        }
    }
}
