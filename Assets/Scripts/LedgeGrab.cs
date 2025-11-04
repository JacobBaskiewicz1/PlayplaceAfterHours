using UnityEngine;

public class LedgeGrab : MonoBehaviour, IInteractable
{
    public float interactRange = 3.0f;
    public Transform climbPosition;
    public float climbSpeed = 3f;
    public void Interact()
    {

    }
    
    public float GetInteractRange()
    {
        return interactRange;
    }
}
