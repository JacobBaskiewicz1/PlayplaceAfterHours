using UnityEngine;

public class Button : MonoBehaviour, IInteractable
{
    public float interactRange = 3.0f;

    public float GetInteractRange()
    {
        return interactRange;
    }

    public void Interact()
    {
        Debug.Log("Button hit!!!");
    }
}
