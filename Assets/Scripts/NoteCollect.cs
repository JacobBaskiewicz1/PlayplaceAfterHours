using System.Linq.Expressions;
using UnityEngine;

public class NoteCollect : MonoBehaviour, IInteractable
{
    public FirstPersonController player;
    public int noteNum;
    public float interactRange = 3.0f;
    // note 1
    [SerializeField] private GameObject noteOneUI;
    [SerializeField] private GameObject noteOneButton;

    // note 2
    [SerializeField] private GameObject noteTwoUI;
    [SerializeField] private GameObject noteTwoButton;

    // note 3
    [SerializeField] private GameObject noteThreeUI;
    [SerializeField] private GameObject noteThreeButton;

    public void Interact()
    {
        if (noteNum == 1)
        {
            EnableUI(noteOneUI);
            noteOneButton.SetActive(true);
            Destroy(gameObject);
        }
        else if (noteNum == 2)
        {
            EnableUI(noteTwoUI);
            noteTwoButton.SetActive(true);
            Destroy(gameObject);
        }
        else if (noteNum == 3)
        {
            EnableUI(noteThreeUI);
            noteThreeButton.SetActive(true);
            Destroy(gameObject);
        }
    }

    public float GetInteractRange()
    {
        return interactRange;
    }
    
    public void EnableUI(GameObject ui)
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
        ui.SetActive(true);
        player.cameraCanMove = false;
    }
}
