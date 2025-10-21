using UnityEngine;
using System.Collections;

public class MonkeyBars : MonoBehaviour, IInteractable
{
    public bool isHolding = false;
    FirstPersonController player;
     private Rigidbody rb; // player rigidbody
    private bool hasJumped = false;

    public float hangOffset = 1.2f;
    public float jumpBoost = 15f;

    // get player obj
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonController>();
        rb = player.GetComponent<Rigidbody>();
    }

    // on interaction with monkey bars
    public void Interact()
    {
        if (!isHolding)
        {
            // start holding
            isHolding = true;
            hasJumped = false;

            player.playerCanMove = false; // disable movement
            player.enableJump = true; // enable jump
            player.enableHeadBob = false; // disable headbob
            player.NotGrounded(); // set not grounded in player controller

            // disable gravity and remove all velocity from player
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;


            // set player just beneath monkey bars
            Vector3 hangPosition = new Vector3(transform.position.x, transform.position.y - hangOffset, transform.position.z);
            player.transform.position = Vector3.Lerp(player.transform.position, hangPosition, 0.5f);

            Debug.Log("grabbed monkey bars");
        }
    }

    // update
    void Update()
    {
        // if holding and not jumped
        if (isHolding && !hasJumped)
        {
            // detect jump
            if (Input.GetKeyDown(player.jumpKey))
            {
                ReleaseFromBars();
            }
        }
    }

    // release from bars
    private void ReleaseFromBars()
    {
        hasJumped = true;
        isHolding = false;

        // Re-enable movement and other player features
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        player.playerCanMove = true;
        player.enableJump = false;
        player.enableHeadBob = true;

        // Proper reset of physics
        rb.useGravity = true;
        rb.linearVelocity = Vector3.zero; // FULL reset before applying new force
        rb.angularVelocity = Vector3.zero;

        // Force a small delay to let physics re-enable gravity cleanly
        StartCoroutine(ApplyJumpForceNextFrame());
    }

    private IEnumerator ApplyJumpForceNextFrame()
    {
        yield return new WaitForFixedUpdate(); // wait for physics step

        // Add jump boost in a consistent direction
        Vector3 jumpDir = player.transform.forward * 2f + Vector3.up;
        rb.linearVelocity = jumpDir.normalized * jumpBoost;

        Debug.Log("released from bars");
    }
}
