using Unity.VisualScripting;
using UnityEngine;

interface IInteractable // interactable interface for interactable objects
{
    public void Interact();
}
public class Interactor : MonoBehaviour
{
    public Transform interactorSource;
    public float interactRange;
    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) // on E press
        {
            Camera cam = Camera.main;
            Ray r = new Ray(cam.transform.position, cam.transform.forward); // create ray towrads camera direciton 
            if (Physics.Raycast(r, out RaycastHit hitInfo, interactRange)) // cast ray
            {   // see if ray has hit obj, if so return obj that contains IInteractable interface
                if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
                {
                    interactObj.Interact(); // if so, interact
                }
            }
            Debug.DrawRay(interactorSource.position, interactorSource.forward * interactRange, Color.red, 0.1f);
        }
    }
}
