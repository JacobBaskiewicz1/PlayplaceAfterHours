using UnityEngine;

public class EndGameCollider : MonoBehaviour
{
    private string playerTag = "Player";
    private bool hasActivated = false;
    [SerializeField] private UIManager uim;
    FirstPersonController player;
    [SerializeField] private AudioSource victoryAudio;
    [SerializeField] private AudioManager am;
    [SerializeField] private GameObject getOutText;

    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonController>();
    }

    // end game UI
    [SerializeField] private GameObject endScreenUI;

    private void OnTriggerEnter(Collider other)
    {
        if (hasActivated) return;                 // Only run once
        if (!other.CompareTag(playerTag)) return; // Only player triggers it

        hasActivated = true;
        
        EndGame();
    }

    private void EndGame()
    {
        endScreenUI.SetActive(true);
        getOutText.SetActive(false);
        am.StopMain();
        am.StopChase();
        victoryAudio.Play();
        player.cameraCanMove = false;
        uim.canPause = false;
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
