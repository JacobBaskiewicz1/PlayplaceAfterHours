using UnityEngine;
using System.Collections;

public class PuzzleInteraction : MonoBehaviour, IInteractable
{
    [Header("Camera Focus")]
    public Transform cameraFocus; // where camera moves to when inspecting puzzle
    public float transitionDuration = 1.2f;

    [Header("UI Panel")]
    public GameObject UIPanel; // Assign your TicTacToePanel here

    public float interactRange = 5.0f;

    private bool inPuzzle = false;
    private bool transitioning = false;

    private FirstPersonController player;
    private Camera playerCamera;
    private Transform cameraParentOriginal;
    private Vector3 cameraLocalPosOriginal;
    private Quaternion cameraLocalRotOriginal;
    private MeshRenderer[] playerMeshes;

    public float GetInteractRange() => interactRange;
    public bool IsInPuzzle() => inPuzzle;

    [Header("UI Manager")]
    [SerializeField] private UIManager uim;

    void Awake()
    {
        // Ensure the panel is completely hidden at the start
        if (UIPanel != null)
            UIPanel.SetActive(false);
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonController>();
        playerCamera = Camera.main;
        uim.canPause = true;

        // Save original parent and local transform
        cameraParentOriginal = playerCamera.transform.parent;
        cameraLocalPosOriginal = playerCamera.transform.localPosition;
        cameraLocalRotOriginal = playerCamera.transform.localRotation;

        // Get all renderers (player body parts, arms, etc.)
        playerMeshes = player.GetComponentsInChildren<MeshRenderer>();
    }

    void Update()
    {
        // Pressing E exits puzzle mode when already in
        if (inPuzzle && !transitioning && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(ExitPuzzle());
            uim.SetNotInPuzzle();
        }
        if (inPuzzle && !transitioning && Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(ExitPuzzle());
            uim.SetNotInPuzzle();
        }
    }

    public void Interact()
    {
        if (transitioning) return;

        if (!inPuzzle) {
            StartCoroutine(EnterPuzzle());
            uim.SetInPuzzle();
        }
        else {
            StartCoroutine(ExitPuzzle());
            uim.SetNotInPuzzle();
        }
    }

    private IEnumerator EnterPuzzle()
    {
        uim.canPause = false;
        transitioning = true;
        inPuzzle = true;

        // Disable player movement & look
        player.playerCanMove = false;
        player.cameraCanMove = false;
        player.enableHeadBob = false;

        // Make player invisible
        foreach (var r in playerMeshes)
            r.enabled = false;

        // Unlock cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Enable UI panel
        if (UIPanel != null)
            UIPanel.SetActive(true);

        // Smoothly move camera to puzzle focus
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

        // Parent camera to focus so it stays fixed in puzzle space
        cam.SetParent(cameraFocus, true);

        transitioning = false;
        uim.canPause = true;
        Debug.Log("Entered puzzle mode");
    }

    private IEnumerator ExitPuzzle()
    {
        uim.canPause = false;
        transitioning = true;
        inPuzzle = false;

        // Disable UI panel
        if (UIPanel != null)
            UIPanel.SetActive(false);

        // Reparent camera back to player
        Transform cam = playerCamera.transform;
        cam.SetParent(cameraParentOriginal, true);

        // Lock cursor back
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Smoothly return to original local transform relative to player
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

        // Make player visible again
        foreach (var r in playerMeshes)
            r.enabled = true;

        // Re-enable player control cleanly (now that camera is synced)
        player.playerCanMove = true;
        player.cameraCanMove = true;
        player.enableHeadBob = true;

        transitioning = false;
        uim.canPause = true;
        Debug.Log("Exited puzzle mode");
    }

    private float EaseInOutCubic(float t)
    {
        return t < 0.5f
            ? 4 * t * t * t
            : 1 - Mathf.Pow(-2 * t + 2, 3) / 2;
    }
}
