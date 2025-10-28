using UnityEngine;

public class Tile : MonoBehaviour
{
    [Header("0 = empty, 1 = X, 2 = O")]
    public int state = 0;

    public Material emptyMaterial;
    public Material xMaterial;
    public Material oMaterial;

    private Renderer rend;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        if (rend == null)
            rend = GetComponentInChildren<Renderer>();

        UpdateVisual();
    }

    public void Cycle()
    {
        state = (state + 1) % 3;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (rend == null) return;

        switch (state)
        {
            case 0:
                rend.material = emptyMaterial;
                break;
            case 1:
                rend.material = xMaterial;
                break;
            case 2:
                rend.material = oMaterial;
                break;
        }
    }
}
