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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Camera cam = Camera.main;
            Ray r = new Ray(cam.transform.position, cam.transform.forward);

            // Cast with the *maximum* possible range (for puzzles)
            if (Physics.Raycast(r, out RaycastHit hitInfo, 10f)) // adjust 10f as your global max
            {
                if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
                {
                    // Check the object's own range
                    float range = interactObj.GetInteractRange();

                    // Only interact if within that specific range
                    if (Vector3.Distance(cam.transform.position, hitInfo.point) <= range)
                    {
                        interactObj.Interact();
                    }
                }
            }

            Debug.DrawRay(interactorSource.position, interactorSource.forward * defaultRange, Color.red, 0.1f);
        }
    }
}

