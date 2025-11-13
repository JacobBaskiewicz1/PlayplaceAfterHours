using UnityEngine;

public class Key : MonoBehaviour, IInteractable
{
    public float interactRange = 3.0f;
    public static bool hasKey = false;
    [SerializeField] private AudioSource pickupSound;
    public float GetInteractRange()
    {
        return interactRange;
    }
    public void Interact()
    {
        hasKey = true;
        AudioSource.PlayClipAtPoint(pickupSound.clip, transform.position);
        Destroy(gameObject);
        Debug.Log("got key!");
    }
}
