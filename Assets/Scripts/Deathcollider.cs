using System;
using UnityEngine;

public class Deathcollider : MonoBehaviour
{
    private DeathScreenManager deathScreenManager;

    private void Start()
    {
        deathScreenManager = FindFirstObjectByType<DeathScreenManager>();
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("dead");
            deathScreenManager.ShowDeathScreen();
        }
    }
}
