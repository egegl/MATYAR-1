using UnityEngine;

public class LimitFps : MonoBehaviour
{
    void Awake()
    {
        Application.targetFrameRate = 60;
    }
}
