using UnityEngine;

public class Flashlight : MonoBehaviour
{
    public GameObject flashlightLight;
    public AudioSource flick;
    private bool isOn = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            isOn = !isOn;
            flashlightLight.SetActive(isOn);
            flick.Play();
        }
    }
}
