using UnityEngine;

public class FakeInteraction : MonoBehaviour, IInteractable
{
    public float interactRange = 3.0f;
    public float GetInteractRange()
    {
        return interactRange;
    }

    public void Interact()
    {
        return;
    }
}
