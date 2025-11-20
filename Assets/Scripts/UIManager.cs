using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject pauseMenu;
    public FirstPersonController player;
    public bool canPause = true;
    public bool inPuzzle = false;

    void Update()
    {   
        if (inPuzzle) return;
        
        if (canPause && Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }

    public void Pause()
    {
        canPause = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
        player.cameraCanMove = false;
    }

    public void Return()
    {
        canPause = true;
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

    public void Play()
    {
        SceneManager.LoadScene(1); 
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0); 
    }

    public void SetInPuzzle()
    {
        inPuzzle = true;
    }
    public void SetNotInPuzzle()
    {
        inPuzzle = false;
    }
    public bool PlayerInPuzzle()
    {
        return inPuzzle;
    }
}
