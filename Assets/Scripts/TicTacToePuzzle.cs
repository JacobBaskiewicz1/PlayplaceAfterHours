using UnityEngine;
using System.Collections;

public class TicTacToePuzzle : MonoBehaviour, IInteractable
{
    [Header("Puzzle Settings")]
    public GameObject doorToOpen;
    public float doorSlideAmount = 3f;
    public float doorSlideSpeed = 2f;

    [Header("Tiles (assign all 9 tile GameObjects in order)")]
    public GameObject[] tiles;  // Each tile should have a Tile component attached

    [Header("Solution Pattern (0 = empty, 1 = X, 2 = O)")]
    public int[] correctPattern = new int[9];

    private bool puzzleSolved = false;
    private PuzzleInteraction puzzleInteraction;

    // IInteractable requirement (used by Interactor)
    public float interactRange = 5f;
    public float GetInteractRange() { return interactRange; }

    private void Start()
    {
        puzzleInteraction = GetComponent<PuzzleInteraction>();

        if (puzzleInteraction == null)
        {
            Debug.LogError("TicTacToePuzzle requires a PuzzleInteraction component on the same GameObject!");
        }
    }

    /// <summary>
    /// Call this method from a UI Button's OnClick event.
    /// Pass the tile index (0-8) for the button.
    /// </summary>
    public void OnTileButtonClicked(int tileIndex)
    {
        if (puzzleSolved) return;
        if (tileIndex < 0 || tileIndex >= tiles.Length) return;

        // Only allow interaction if the puzzle is currently active
        if (puzzleInteraction == null || !puzzleInteraction.IsInPuzzle()) return;

        Tile tile = tiles[tileIndex].GetComponent<Tile>();
        if (tile != null)
        {
            tile.Cycle();
            CheckPuzzleSolved();
        }
    }

    private void CheckPuzzleSolved()
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            Tile t = tiles[i].GetComponent<Tile>();
            if (t == null) return;

            if (t.state != correctPattern[i])
                return; // not solved
        }

        puzzleSolved = true;
        Debug.Log("Puzzle solved!");

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
        Debug.Log("Interacted with TicTacToe puzzle");
    }
}
