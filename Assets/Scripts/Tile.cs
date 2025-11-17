using UnityEngine;

public class Tile : MonoBehaviour
{
    [Header("0 = O, 1 = blank, 2 = X, 3 = blank")]
    public int state = 0;

    /// <summary>
    /// Cycle the tile to the next state
    /// </summary>
    public void Cycle()
    {
        state = (state + 1) % 4;
    }
}
