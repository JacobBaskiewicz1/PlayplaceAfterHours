using UnityEngine;
using System.Collections;

public class AbacusPuzzle : MonoBehaviour, IInteractable
{
    [Header("Puzzle Settings")]
    public GameObject doorToOpen;
    public float doorSlideAmount = 3f;
    public float doorSlideSpeed = 2f;

    [Header("Rows (assign all 5 row GameObjects in order)")]
    public BeadRow[] rows;  // Each row should have a BeadRow component

    [Header("Solution Pattern (number of beads on left for each row)")]
    public int[] correctPattern = new int[5];  // e.g., {4, 3, 7, 1, 2}

    private bool puzzleSolved = false;
    private PuzzleInteraction puzzleInteraction;

    // IInteractable requirement
    public float interactRange = 5f;
    public float GetInteractRange() => interactRange;

    private void Start()
    {
        puzzleInteraction = GetComponent<PuzzleInteraction>();

        if (puzzleInteraction == null)
        {
            Debug.LogError("AbacusPuzzle requires a PuzzleInteraction component on the same GameObject!");
        }
    }

    /// <summary>
    /// Call from UI buttons. RowIndex = 0-4
    /// </summary>
    public void OnMoveRowLeft(int rowIndex)
    {
        if (puzzleSolved) return;
        if (rowIndex < 0 || rowIndex >= rows.Length) return;
        if (puzzleInteraction == null || !puzzleInteraction.IsInPuzzle()) return;

        rows[rowIndex].MoveBeadLeft();
        CheckPuzzleSolved();
    }

    /// <summary>
    /// Call from UI buttons. RowIndex = 0-4
    /// </summary>
    public void OnMoveRowRight(int rowIndex)
    {
        if (puzzleSolved) return;
        if (rowIndex < 0 || rowIndex >= rows.Length) return;
        if (puzzleInteraction == null || !puzzleInteraction.IsInPuzzle()) return;

        rows[rowIndex].MoveBeadRight();
        CheckPuzzleSolved();
    }

    private void CheckPuzzleSolved()
    {
        for (int i = 0; i < rows.Length; i++)
        {
            if (rows[i] == null) return;

            if (rows[i].beadsOnLeft != correctPattern[i])
                return; // Not solved
        }

        puzzleSolved = true;
        Debug.Log("Abacus puzzle solved!");

        if (doorToOpen != null)
            StartCoroutine(SlideDoorDown());
    }

    private IEnumerator SlideDoorDown()
    {
        Vector3 start = doorToOpen.transform.position;
        Vector3 end = start - new Vector3(0, doorSlideAmount, 0);

        while (Vector3.Distance(doorToOpen.transform.position, end) > 0.01f)
        {
            doorToOpen.transform.position = Vector3.MoveTowards(
                doorToOpen.transform.position,
                end,
                doorSlideSpeed * Time.deltaTime
            );
            yield return null;
        }
    }

    public void Interact()
    {
        Debug.Log("Interacted with Abacus puzzle");
    }
}
