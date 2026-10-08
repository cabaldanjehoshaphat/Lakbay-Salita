using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// CrosswordAnswerKey
/// Debug helper: lists which data script the current puzzle came from (e.g. "Cebuano_Level5")
/// plus every Across and Down answer, right in the Inspector, so a tester can check solutions
/// without opening the puzzle data file. Attached to EventSystem purely as a convenient,
/// always-present place to see it - it has no gameplay effect.
///
/// CrosswordInputController picks a random level each time it (re)generates and calls
/// RefreshFrom() with the result, so this always reflects whichever puzzle actually got
/// picked - not a hardcoded one. "Refresh Answers" re-pulls from whatever
/// CrosswordInputController currently has loaded (or shows a placeholder if nothing has
/// generated yet in this session).
/// </summary>
public class CrosswordAnswerKey : MonoBehaviour
{
    [Header("Puzzle Source")]
    [SerializeField] private string puzzleSource = "(not generated yet)";

    [Header("Across")]
    [SerializeField] private List<string> across = new List<string>();

    [Header("Down")]
    [SerializeField] private List<string> down = new List<string>();

    [ContextMenu("Refresh Answers")]
    private void RefreshFromScene()
    {
        CrosswordInputController controller = FindObjectOfType<CrosswordInputController>();
        if (controller == null || controller.CurrentPuzzle == null)
        {
            puzzleSource = "(not generated yet - press Play, or use CrosswordInputController's \"Generate Puzzle In Editor\")";
            across.Clear();
            down.Clear();
            return;
        }

        RefreshFrom(controller.CurrentPuzzle, controller.CurrentPuzzleSourceName);
    }

    /// <summary>Called by CrosswordInputController right after it picks and builds a puzzle,
    /// so this Inspector view stays in sync without polling.</summary>
    public void RefreshFrom(CrosswordPuzzle puzzle, string sourceName)
    {
        puzzleSource = sourceName;

        across = puzzle.words
            .Where(w => w.direction == CrosswordDirection.Across)
            .OrderBy(w => w.number)
            .Select(w => $"{w.number}. {w.answer} - {w.clue}")
            .ToList();

        down = puzzle.words
            .Where(w => w.direction == CrosswordDirection.Down)
            .OrderBy(w => w.number)
            .Select(w => $"{w.number}. {w.answer} - {w.clue}")
            .ToList();
    }
}
