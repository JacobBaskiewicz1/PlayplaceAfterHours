using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource footsteps;
    FirstPersonController player;
    private PuzzleInteraction[] puzzles;
    [SerializeField] private AudioSource chaseTheme;
    [SerializeField] private AudioSource mainTheme;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonController>();
        puzzles = FindObjectsByType<PuzzleInteraction>(FindObjectsSortMode.None);
        PlayMain();
    }
    void Update()
    {
        // check when paused or in puzzle to stop playing when true
        if (Time.timeScale == 0f || IsInAnyPuzzle())
        {
            footsteps.enabled = false;
            return;
        }

        if(player.PlayerIsWalking() && player.PlayerIsGrounded())
        {
            footsteps.enabled = true;
        } else
        {
            footsteps.enabled = false;
        }
    }
    bool IsInAnyPuzzle()
    {
        foreach (var puzzle in puzzles)
        {
            if (puzzle != null && puzzle.IsInPuzzle())
                return true;
        }
        return false;
    }

    public void PlayMain()
    {
        mainTheme.Play();
    }
    public void StopMain()
    {
        mainTheme.Stop();
    }

    public void PlayChase()
    {
        chaseTheme.Play();
    }
    public void StopChase()
    {
        chaseTheme.Stop();
    }
}
