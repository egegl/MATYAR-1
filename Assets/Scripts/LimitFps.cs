using UnityEngine;

public class LimitFps : MonoBehaviour
{
    void Start()
    {
        Application.targetFrameRate = 60;
    }
}
