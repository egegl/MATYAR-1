using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouseTakipScript : MonoBehaviour
{
    public Transform tr;

    void Start()
    {
        
    }

    void Update()
    {
        tr.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
