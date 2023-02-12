using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Clock : MonoBehaviour
{  
    public Clock Instance { get; private set; }

    public void Awake()
    {
        Instance = this;
    }
}
