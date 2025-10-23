using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource footsteps;
    FirstPersonController player;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonController>();
    }
    void Update()
    {
        if(player.PlayerIsWalking() && player.PlayerIsGrounded())
        {
            footsteps.enabled = true;
        } else
        {
            footsteps.enabled = false;
        }
    }
}
