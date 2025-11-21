using UnityEngine;

public class YellowTube2 : MonoBehaviour
{
    [SerializeField] private GameObject walla;
    [SerializeField] private GameObject endCollider;
    [SerializeField] private GameObject slideDoor;
    [SerializeField] private GameObject getOutText;
    private string playerTag = "Player";
    private bool hasActivated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasActivated) return;                 // Only run once
        if (!other.CompareTag(playerTag)) return; // Only player triggers it

        Debug.Log("got here");
        walla.SetActive(true);
        endCollider.SetActive(true);
        getOutText.SetActive(true);
        slideDoor.SetActive(false);
        hasActivated = true;
    }
}
