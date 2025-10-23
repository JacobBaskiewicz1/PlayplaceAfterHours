using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreenManager : MonoBehaviour
{
    public GameObject deathScreenUI;
    FirstPersonController player;
    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonController>();
    }

    public void ShowDeathScreen()
    {
        // activate ui, freeze game, unlock cursor, freeze camera
        player.cameraCanMove = false;
        deathScreenUI.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // restart scene
    public void RestartLevel()
    {
        player.cameraCanMove = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}