using UnityEngine;
using System.Collections;

public class LedgeGrab : MonoBehaviour, IInteractable
{
    public float interactRange = 3.0f;
    public Transform climbPosition;
    public float climbSpeed = 3f;
    public FirstPersonController player;
    public Transform playerPos;
    public void Interact()
    {
        if (player != null)
        {
            player.StartCoroutine(ClimbPlayer(playerPos));
        }
    }

    public float GetInteractRange()
    {
        return interactRange;
    }
    
    private IEnumerator ClimbPlayer(Transform playerPos)
    {
        player.playerCanMove = false; // disable movement
        player.enableHeadBob = false; // disable headbob
        player.NotGrounded(); // set not grounded in player controller

        Vector3 startPos = playerPos.position;
        Vector3 endPos = climbPosition.position;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * climbSpeed;
            playerPos.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        // Re-enable player movement
        player.playerCanMove = true;
        player.enableHeadBob = true;
        player.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
    }
}
