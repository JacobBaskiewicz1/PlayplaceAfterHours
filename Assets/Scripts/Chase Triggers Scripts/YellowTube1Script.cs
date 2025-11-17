using UnityEngine;
using System.Collections;

public class YellowTube1Script : MonoBehaviour
{
    [Header("Door Settings")]
    public GameObject doorObject;                 // Door to move
    public Vector3 slideDirection = Vector3.down; // Direction of movement
    public float slideAmount = 3f;                // Distance to move
    public float slideSpeed = 2f;                 // Speed of movement

    [Header("Player Tag")]
    public string playerTag = "Player";           // Must match your player tag

    private bool hasActivated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasActivated) return;                 // Only run once
        if (!other.CompareTag(playerTag)) return; // Only player triggers it
        if (doorObject == null) return;

        hasActivated = true;
        StartCoroutine(SlideDoor());
    }

    private IEnumerator SlideDoor()
    {
        Vector3 start = doorObject.transform.position;
        Vector3 end = start + (slideDirection.normalized * slideAmount);

        while (Vector3.Distance(doorObject.transform.position, end) > 0.01f)
        {
            doorObject.transform.position = Vector3.MoveTowards(
                doorObject.transform.position,
                end,
                slideSpeed * Time.deltaTime
            );
            yield return null;
        }

        doorObject.transform.position = end;
    }
}
