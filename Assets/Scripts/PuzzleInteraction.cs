using UnityEngine;
using System.Collections;

public class PuzzleInteraction : MonoBehaviour, IInteractable
{
    [Header("Camera Focus")]
    public Transform cameraFocus; // where camera moves to when inspecting puzzle
    public float transitionDuration = 1.2f;

    public float interactRange = 5.0f;

    private bool inPuzzle = false;
    private bool transitioning = false;

    private FirstPersonController player;
    private Camera playerCamera;
    private Transform cameraParentOriginal;
    private Vector3 cameraLocalPosOriginal;
    private Quaternion cameraLocalRotOriginal;
    private MeshRenderer[] playerMeshes;

    public float GetInteractRange()
    {
        return interactRange;
    }
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonController>();
        playerCamera = Camera.main;

        // save original parent and local transform
        cameraParentOriginal = playerCamera.transform.parent;
        cameraLocalPosOriginal = playerCamera.transform.localPosition;
        cameraLocalRotOriginal = playerCamera.transform.localRotation;

        // get all renderers (player body parts, arms, etc.)
        playerMeshes = player.GetComponentsInChildren<MeshRenderer>();
    }

    void Update()
    {
        // pressing E exits puzzle mode when already in
        if (inPuzzle && !transitioning && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(ExitPuzzle());
        }
    }

    public void Interact()
    {
        if (transitioning) return;

        if (!inPuzzle)
            StartCoroutine(EnterPuzzle());
        else
            StartCoroutine(ExitPuzzle());
    }

    private IEnumerator EnterPuzzle()
    {
        transitioning = true;
        inPuzzle = true;

        // disable player movement & look
        player.playerCanMove = false;
        player.cameraCanMove = false;
        player.enableHeadBob = false;
        player.enableJump = false;

        // make player invisible
        foreach (var r in playerMeshes)
            r.enabled = false;

        // unlock cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // smoothly move camera to puzzle focus
        Transform cam = playerCamera.transform;
        Vector3 startPos = cam.position;
        Quaternion startRot = cam.rotation;
        Vector3 targetPos = cameraFocus.position;
        Quaternion targetRot = cameraFocus.rotation;

        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / transitionDuration);
            float eased = EaseInOutCubic(t);
            cam.position = Vector3.Lerp(startPos, targetPos, eased);
            cam.rotation = Quaternion.Slerp(startRot, targetRot, eased);
            yield return null;
        }

        // parent camera to focus so it stays fixed in puzzle space
        cam.SetParent(cameraFocus, true);

        transitioning = false;
        Debug.Log("Entered puzzle mode");
    }

    private IEnumerator ExitPuzzle()
    {
        transitioning = true;
        inPuzzle = false;

        // reparent camera back to player first
        Transform cam = playerCamera.transform;
        cam.SetParent(cameraParentOriginal, true);

        // lock cursor back
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // smoothly return to original local transform relative to player
        Vector3 startPos = cam.localPosition;
        Quaternion startRot = cam.localRotation;
        Vector3 targetPos = cameraLocalPosOriginal;
        Quaternion targetRot = cameraLocalRotOriginal;

        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / transitionDuration);
            float eased = EaseInOutCubic(t);
            cam.localPosition = Vector3.Lerp(startPos, targetPos, eased);
            cam.localRotation = Quaternion.Slerp(startRot, targetRot, eased);
            yield return null;
        }

        // make player visible again
        foreach (var r in playerMeshes)
            r.enabled = true;

        // re-enable player control cleanly (now that camera is synced)
        player.playerCanMove = true;
        player.cameraCanMove = true;
        player.enableHeadBob = true;
        player.enableJump = true;

        transitioning = false;
        Debug.Log("Exited puzzle mode");
    }

    // cubic ease for smooth transitions
    private float EaseInOutCubic(float t)
    {
        return t < 0.5f
            ? 4 * t * t * t
            : 1 - Mathf.Pow(-2 * t + 2, 3) / 2;
    }
}
