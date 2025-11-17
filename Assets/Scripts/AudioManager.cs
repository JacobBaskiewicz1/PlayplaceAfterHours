using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource footsteps;
    FirstPersonController player;
    private PuzzleInteraction[] puzzles;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonController>();
        puzzles = FindObjectsByType<PuzzleInteraction>(FindObjectsSortMode.None);
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
}
