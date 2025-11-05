using UnityEngine;
using System.Collections;

public class BeadRow : MonoBehaviour
{
    [Header("Bead Settings")]
    public GameObject[] beads;  // All 10 beads in this row
    public int beadsOnLeft = 0;  // Current number of beads on the left
    public float beadSpacing = 0.2f;  // Distance between beads
    public float moveSpeed = 2f;  // Speed of bead movement

    private bool isMoving = false;
    private float leftmostZ = -1.583f;  // Z position of first bead on left (local)
    private float rightmostZ = 1.583f;  // Z position of first bead on right (local, from 7.573 - 5.99)

    private void Start()
    {
        // Initialize bead positions based on beadsOnLeft
        UpdateBeadPositions(immediate: true);
    }

    // Move one bead from right to left
    public void MoveBeadLeft()
    {
        if (isMoving) return;
        if (beadsOnLeft >= 10) return;  // All beads already on left

        beadsOnLeft++;
        StartCoroutine(AnimateBeads());
    }

    // Move one bead from left to right
    public void MoveBeadRight()
    {
        if (isMoving) return;
        if (beadsOnLeft <= 0) return;  // All beads already on right

        beadsOnLeft--;
        StartCoroutine(AnimateBeads());
    }

    private IEnumerator AnimateBeads()
    {
        isMoving = true;

        // Calculate target positions for all beads
        Vector3[] targetPositions = new Vector3[beads.Length];
        for (int i = 0; i < beads.Length; i++)
        {
            targetPositions[i] = GetBeadTargetPosition(i);
        }

        // Animate all beads to their target positions
        bool anyBeadMoving = true;
        while (anyBeadMoving)
        {
            anyBeadMoving = false;

            for (int i = 0; i < beads.Length; i++)
            {
                if (beads[i] == null) continue;

                Vector3 currentPos = beads[i].transform.localPosition;
                Vector3 targetPos = targetPositions[i];

                if (Vector3.Distance(currentPos, targetPos) > 0.01f)
                {
                    beads[i].transform.localPosition = Vector3.MoveTowards(
                        currentPos,
                        targetPos,
                        moveSpeed * Time.deltaTime
                    );
                    anyBeadMoving = true;
                }
                else
                {
                    beads[i].transform.localPosition = targetPos;
                }
            }

            yield return null;
        }

        isMoving = false;
    }

    private void UpdateBeadPositions(bool immediate)
    {
        for (int i = 0; i < beads.Length; i++)
        {
            if (beads[i] == null) continue;

            Vector3 targetPos = GetBeadTargetPosition(i);

            if (immediate)
            {
                beads[i].transform.localPosition = targetPos;
            }
        }
    }

    private Vector3 GetBeadTargetPosition(int beadIndex)
    {
        float targetZ;

        // Beads are arranged from left to right: 0, 1, 2, 3, 4, 5, 6, 7, 8, 9
        // Left group: beads 0 to (beadsOnLeft-1) go on the left side
        // Right group: beads beadsOnLeft to 9 go on the right side

        if (beadIndex < beadsOnLeft)
        {
            // This bead is in the left group
            // Position from leftmost, spaced by beadSpacing
            targetZ = leftmostZ + (beadIndex * beadSpacing);
        }
        else
        {
            // This bead is in the right group
            // Position from rightmost, going left by beadSpacing
            int positionFromRight = (9 - beadIndex);  // 0 is rightmost
            targetZ = rightmostZ - (positionFromRight * beadSpacing);
        }

        // Keep X and Y the same, only change Z
        Vector3 currentPos = beads[beadIndex].transform.localPosition;
        return new Vector3(currentPos.x, currentPos.y, targetZ);
    }

    public bool IsMoving()
    {
        return isMoving;
    }
}