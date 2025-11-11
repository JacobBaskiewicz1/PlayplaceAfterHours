using UnityEngine;
public class InteractibleDoor : MonoBehaviour, IInteractable
{
    public float interactRange = 3.0f;
    public Animator doorAnimator;
    [SerializeField] private AudioSource openSound;
    private bool doorOpen = false;
    public float GetInteractRange()
    {
        return interactRange;
    }

    public void Interact()
    {
        if (Key.hasKey && !doorOpen)
        {
            if (doorAnimator != null)
            {
                AudioSource.PlayClipAtPoint(openSound.clip, transform.position);
                doorAnimator.Play("doorOpen");
                doorOpen = true;
            }
            else
            {
                Debug.Log("Door Opened");
                gameObject.SetActive(false); // simple door removal
                doorOpen = true;
            }
        }
        else
        {
            Debug.Log("The door is locked.");
        }
    }
}