using UnityEngine;

/// <summary>
/// CrosswordBlackCell
/// Marker added (at runtime, by CrosswordGridGenerator) to every black/blocked cell it
/// instantiates, recording its grid position. Black cells otherwise have no script of
/// their own - this exists purely so CrosswordGridGenerator can reposition/resize them
/// later (e.g. when cellSize or spacing changes in the Inspector) without having to
/// rebuild the whole grid from puzzle data.
/// </summary>
public class CrosswordBlackCell : MonoBehaviour
{
    public int Row { get; private set; }
    public int Col { get; private set; }

    public void Initialize(int row, int col)
    {
        Row = row;
        Col = col;
    }
}
