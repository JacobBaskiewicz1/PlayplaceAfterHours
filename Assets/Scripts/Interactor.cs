using Unity.VisualScripting;
using UnityEngine;

interface IInteractable // interactable interface for interactable objects
{
    public void Interact();
    float GetInteractRange();
}
public class Interactor : MonoBehaviour
{
    public Transform interactorSource;
    public float defaultRange = 3f; // fallback range if none provided

    [Header("UI Prompt")]
    public GameObject interactPrompt;
    private IInteractable currentLookTarget = null;
    
    [Header("UI Manager")]
    [SerializeField] private UIManager uim;

    void Update()
    {
        if (uim.PlayerInPuzzle()) {
            interactPrompt.SetActive(false);
            return;
        }
        HandleLookForInteractable();
        HandleInteraction();
    }

    void HandleLookForInteractable()
    {
        Camera cam = Camera.main;
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        // Perform a raycast to detect interactable objects
        if (Physics.Raycast(ray, out RaycastHit hit, 10f))
        {
            if (hit.collider.TryGetComponent(out IInteractable interactObj))
            {
                float objRange = interactObj.GetInteractRange();
                float dist = Vector3.Distance(cam.transform.position, hit.point);

                if (dist <= objRange)
                {
                    // Player is looking at and within range of an interactable object
                    currentLookTarget = interactObj;

                    if (interactPrompt != null && !interactPrompt.activeSelf)
                        interactPrompt.SetActive(true);

                    return;
                }
            }
        }

        currentLookTarget = null;
        if (interactPrompt != null && interactPrompt.activeSelf)
            interactPrompt.SetActive(false);
    }
    void HandleInteraction()
    {
        if (Input.GetKeyDown(KeyCode.E) && currentLookTarget != null)
        {
            currentLookTarget.Interact();
        }
    }
}

