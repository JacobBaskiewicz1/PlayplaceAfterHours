using UnityEngine;
using System.Collections;

public class LookAtKiller : MonoBehaviour
{
    [Header("Killer Settings")]
    public Transform killerTransform;              // The killer object to look at
    public float lookDuration = 2f;                // How long to look at killer
    public float rotationSpeed = 3f;               // Speed of camera rotation

    [Header("Chase Sequence Settings")]
    public Transform[] chasePoints;                // Array of positions for killer to move through
    public float killerMoveSpeed = 5f;             // Speed at which killer moves between points
    public float killerRotationSpeed = 5f;         // Speed at which killer rotates toward next point
    public float killerRotationOffset = 0f;        // Offset to correct model's forward direction (in degrees)
    public bool destroyKillerAtEnd = true;         // Whether to destroy killer at final point

    [Header("Door Settings")]
    public GameObject doorObject;                  // Door to move
    public Vector3 slideDirection = Vector3.down;  // Direction of movement
    public float slideAmount = 3f;                 // Distance to move
    public float slideSpeed = 2f;                  // Speed of movement

    [Header("Player Settings")]
    public string playerTag = "Player";            // Must match your player tag

    private bool hasActivated = false;
    private FirstPersonController playerController;

    [Header("Audio Manager")]
    [SerializeField] AudioManager am;

    private void OnTriggerEnter(Collider other)
    {
        if (hasActivated) return;                  // Only run once
        if (!other.CompareTag(playerTag)) return;  // Only player triggers it

        // Get the FirstPersonController component
        playerController = other.GetComponent<FirstPersonController>();
        if (playerController == null) return;

        if (killerTransform == null)
        {
            Debug.LogWarning("Killer Transform not assigned!");
            return;
        }
        am.StopMain();

        hasActivated = true;
        StartCoroutine(LookAtKillerSequence());
    }

    private IEnumerator LookAtKillerSequence()
    {
        // Store original control states
        bool originalCameraCanMove = playerController.cameraCanMove;
        bool originalPlayerCanMove = playerController.playerCanMove;

        // Lock player controls
        playerController.cameraCanMove = false;
        playerController.playerCanMove = false;

        // Freeze the rigidbody to prevent any movement
        Rigidbody rb = playerController.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        // Get references to player and camera
        Transform playerTransform = playerController.transform;
        Transform cameraTransform = playerController.playerCamera.transform;

        // Calculate target rotation for body (Y axis only)
        Vector3 directionToKiller = killerTransform.position - playerTransform.position;
        directionToKiller.y = 0; // Keep on horizontal plane
        Quaternion targetBodyRotation = Quaternion.LookRotation(directionToKiller);

        // Calculate target rotation for camera (pitch)
        Vector3 cameraToKiller = killerTransform.position - cameraTransform.position;
        float targetPitch = -Mathf.Atan2(cameraToKiller.y,
            new Vector3(cameraToKiller.x, 0, cameraToKiller.z).magnitude) * Mathf.Rad2Deg;

        // Store original rotations
        Quaternion originalBodyRotation = playerTransform.rotation;
        float originalPitch = cameraTransform.localEulerAngles.x;
        if (originalPitch > 180) originalPitch -= 360; // Convert to -180 to 180 range

        // Smoothly rotate to look at killer
        float elapsed = 0f;
        float rotationTime = 1f / rotationSpeed;

        while (elapsed < rotationTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / rotationTime;

            // Rotate body (Y axis)
            playerTransform.rotation = Quaternion.Slerp(originalBodyRotation, targetBodyRotation, t);

            // Rotate camera (X axis - pitch)
            float currentPitch = Mathf.Lerp(originalPitch, targetPitch, t);
            cameraTransform.localEulerAngles = new Vector3(currentPitch, 0, 0);

            yield return null;
        }

        // Ensure we're exactly at target
        playerTransform.rotation = targetBodyRotation;
        cameraTransform.localEulerAngles = new Vector3(targetPitch, 0, 0);

        // Start closing the door while looking at killer
        if (doorObject != null)
        {
            StartCoroutine(SlideDoor());
        }

        // Hold the look
        yield return new WaitForSeconds(lookDuration);

        // Unfreeze the rigidbody
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation; // Only freeze rotation, allow movement
        }

        // Return player control
        playerController.cameraCanMove = true;
        playerController.playerCanMove = true;

        // Start the chase sequence
        if (chasePoints != null && chasePoints.Length > 0)
        {
            StartCoroutine(ChaseSequence());
        }
    }

    private IEnumerator ChaseSequence()
    {
        am.PlayChase();
        // Move killer through each chase point
        for (int i = 0; i < chasePoints.Length; i++)
        {
            if (chasePoints[i] == null) continue;

            Vector3 targetPosition = chasePoints[i].position;

            // Calculate direction to target for rotation (only horizontal)
            Vector3 directionToTarget = targetPosition - killerTransform.position;
            directionToTarget.y = 0; // Keep on horizontal plane - this prevents looking up/down

            if (directionToTarget.sqrMagnitude > 0.001f) // Check if there's a direction to rotate to
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

                // Apply rotation offset to correct model's forward direction
                targetRotation *= Quaternion.Euler(0, killerRotationOffset, 0);

                // Rotate toward target before/while moving
                while (Quaternion.Angle(killerTransform.rotation, targetRotation) > 1f)
                {
                    killerTransform.rotation = Quaternion.Slerp(
                        killerTransform.rotation,
                        targetRotation,
                        killerRotationSpeed * Time.deltaTime
                    );
                    yield return null;
                }

                // Snap to exact rotation
                killerTransform.rotation = targetRotation;
            }

            // Move killer to the target position
            while (Vector3.Distance(killerTransform.position, targetPosition) > 0.01f)
            {
                killerTransform.position = Vector3.MoveTowards(
                    killerTransform.position,
                    targetPosition,
                    killerMoveSpeed * Time.deltaTime
                );
                yield return null;
            }

            // Snap to exact position
            killerTransform.position = targetPosition;
        }

        // Remove killer after reaching final position
        if (destroyKillerAtEnd && killerTransform != null)
        {
            Destroy(killerTransform.gameObject);
            am.StopChase();
            am.PlayMain();
        }
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