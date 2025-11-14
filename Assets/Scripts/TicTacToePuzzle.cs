using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class DoorConfig
{
    public GameObject doorObject;            // The door to move
    public Vector3 slideDirection = Vector3.down; // Direction of movement
    public float slideAmount = 3f;           // Distance to move
    public float slideSpeed = 2f;            // Speed of movement
}

public class TicTacToePuzzle : MonoBehaviour, IInteractable
{
    [Header("Puzzle Settings")]
    public List<DoorConfig> doorsToOpen; // Supports multiple doors

    [Header("Tiles (assign all 9 cylinder tile GameObjects in order)")]
    public GameObject[] tiles;

    [Header("Solution Pattern (0 = O, 1 = blank, 2 = X, 3 = blank)")]
    public int[] correctPattern = new int[9];

    [Header("Audio")]
    public AudioSource tap;

    private bool puzzleSolved = false;
    private PuzzleInteraction puzzleInteraction;

    // IInteractable interface
    public float interactRange = 5f;
    public float GetInteractRange() { return interactRange; }

    private void Start()
    {
        puzzleInteraction = GetComponent<PuzzleInteraction>();

        if (puzzleInteraction == null)
            Debug.LogError("TicTacToePuzzle requires a PuzzleInteraction component on the same GameObject!");
    }

    /// <summary>
    /// Called by UI button click. Pass the tile index (0–8)
    /// </summary>
    public void OnTileButtonClicked(int tileIndex)
    {
        if (puzzleSolved) return;
        if (tileIndex < 0 || tileIndex >= tiles.Length) return;
        if (puzzleInteraction == null || !puzzleInteraction.IsInPuzzle()) return;

        if (tap != null) tap.Play();

        Tile tile = tiles[tileIndex].GetComponent<Tile>();
        if (tile == null) return;

        // Cycle state and animate rotation
        StartCoroutine(RotateTile(tile));
    }

    /// <summary>
    /// Rotate the tile 90° around Y based on its current state
    /// </summary>
    private IEnumerator RotateTile(Tile tile)
    {
        float startY = tile.transform.localEulerAngles.y;

        tile.Cycle(); // increment state (0–3)
        float targetY = tile.state * 90f; // snap to multiples of 90°

        Quaternion startRot = tile.transform.localRotation;
        Quaternion endRot = Quaternion.Euler(0f, targetY, 0f);

        float duration = 0.25f; // rotation animation duration
        float elapsed = 0f;

        while (elapsed < duration)
        {
            tile.transform.localRotation = Quaternion.Slerp(startRot, endRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        tile.transform.localRotation = endRot; // ensure perfect snap

        CheckPuzzleSolved();
    }

    /// <summary>
    /// Check if all tiles match the correct pattern
    /// </summary>
    private void CheckPuzzleSolved()
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            Tile tile = tiles[i].GetComponent<Tile>();
            if (tile == null) return;

            if (tile.state != correctPattern[i])
                return; // puzzle not solved
        }

        puzzleSolved = true;
        Debug.Log("Puzzle solved!");

        // Open all configured doors
        foreach (DoorConfig door in doorsToOpen)
        {
            if (door.doorObject != null)
                StartCoroutine(SlideDoor(door));
        }
    }

    /// <summary>
    /// Slide a single door in its own direction and distance
    /// </summary>
    private IEnumerator SlideDoor(DoorConfig door)
    {
        Vector3 start = door.doorObject.transform.position;
        Vector3 end = start + (door.slideDirection.normalized * door.slideAmount);

        while (Vector3.Distance(door.doorObject.transform.position, end) > 0.01f)
        {
            door.doorObject.transform.position = Vector3.MoveTowards(
                door.doorObject.transform.position,
                end,
                door.slideSpeed * Time.deltaTime
            );
            yield return null;
        }

        door.doorObject.transform.position = end;
    }

    public void Interact()
    {
        Debug.Log("Interacted with TicTacToe puzzle");
    }
}
