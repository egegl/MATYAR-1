using UnityEngine;

public class Level3Manager : MonoBehaviour
{
    public static Level3Manager Instance { get; private set; }

    private void Awake()
    {
        // singleton
        Instance = this;
    }

    public void ResetLevel()
    {
        return;
    }
}
