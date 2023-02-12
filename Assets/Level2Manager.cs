using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2Manager : MonoBehaviour
{
    public static Level2Manager Instance { get; private set; }

    private void Awake()
    {
        // singleton
        Instance = this;
    }

    // reset level state
    public void ResetLevel()
    {
        Debug.Log("Reset Level 2");
    }
}