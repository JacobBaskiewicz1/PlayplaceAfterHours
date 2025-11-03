using UnityEditor;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject pauseMenu;
    public FirstPersonController player;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }

    public void Pause()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
        player.cameraCanMove = false;
    }

    public void Return()
    {
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        player.cameraCanMove = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("quit");
    }
}
