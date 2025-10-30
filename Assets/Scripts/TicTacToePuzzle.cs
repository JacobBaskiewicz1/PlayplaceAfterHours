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

    [Header("Click Detection")]
    public float tileClickRadius = 0.5f;  // Click radius for each tile

    private bool puzzleSolved = false;
    private PuzzleInteraction puzzleInteraction;

    // IInteractable requirement (used by Interactor)
    public float interactRange = 5f;
    public float GetInteractRange() { return interactRange; }

    private void Start()
    {
        // Get the PuzzleInteraction component on the same GameObject
        puzzleInteraction = GetComponent<PuzzleInteraction>();

        if (puzzleInteraction == null)
        {
            Debug.LogError("TicTacToePuzzle requires a PuzzleInteraction component on the same GameObject!");
        }
    }

    private void Update()
    {
        if (puzzleSolved) return;

        // Only check clicks if cursor is visible AND this puzzle is active
        if (!Cursor.visible) return;

        // Check if THIS puzzle's PuzzleInteraction is active
        if (puzzleInteraction == null || !puzzleInteraction.IsInPuzzle()) return;

        if (Input.GetMouseButtonDown(0))
        {
            DetectTileClick();
        }
    }

    private void DetectTileClick()
    {
        if (tiles.Length == 0) return;

        Camera cam = Camera.main;

        // Use actual screen dimensions, not camera's pixel dimensions
        float viewportX = Input.mousePosition.x / Screen.width;
        float viewportY = Input.mousePosition.y / Screen.height;

        // Convert to -1 to 1 range (centered)
        float horizontalOffset = (viewportX - 0.5f) * 2f;
        float verticalOffset = (viewportY - 0.5f) * 2f;

        // Estimate the screen size in world units at the board's distance
        float distanceToBoard = Vector3.Distance(cam.transform.position, transform.position);
        float verticalFOV = cam.fieldOfView * Mathf.Deg2Rad;
        float screenHeight = 2f * Mathf.Tan(verticalFOV / 2f) * distanceToBoard;

        // Use actual screen aspect ratio, not camera's reported aspect
        float actualAspect = (float)Screen.width / (float)Screen.height;
        float screenWidth = screenHeight * actualAspect;

        // Scale down to account for camera viewport mismatch
        float viewportScale = (float)cam.pixelHeight / (float)Screen.height;
        screenHeight *= viewportScale * 0.7f;
        screenWidth *= viewportScale * 0.7f;

        // Calculate world position on the board plane
        Vector3 cameraToBoard = transform.position - cam.transform.position;
        Vector3 clickOffset = cam.transform.right * (horizontalOffset * screenWidth / 2f) +
                              cam.transform.up * (verticalOffset * screenHeight / 2f);
        Vector3 worldClickPos = cam.transform.position + cameraToBoard + clickOffset;

        // Find closest tile
        Tile closestTile = null;
        float closestDistance = float.MaxValue;
        int closestIndex = -1;

        for (int i = 0; i < tiles.Length; i++)
        {
            GameObject tileObj = tiles[i];
            if (tileObj == null) continue;

            Tile tileScript = tileObj.GetComponent<Tile>();
            if (tileScript == null) continue;

            float distance = Vector3.Distance(worldClickPos, tileObj.transform.position);

            if (distance <= tileClickRadius && distance < closestDistance)
            {
                closestDistance = distance;
                closestTile = tileScript;
                closestIndex = i;
            }
        }

        if (closestTile != null)
        {
            closestTile.Cycle();
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