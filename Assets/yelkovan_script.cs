using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class yelkovan_script : MonoBehaviour
{
    public GameObject akrep;

    public Transform yelkovanOtr;
    public Transform saatMerkez;

    private float angle;
    Vector2 mousePos;
    Vector2 MouseYol;
    Vector2 forwardVector = new Vector2(0, 1);
    Vector2 SmousePos;

    bool onYelkovan = false;
    public bool continueDraggingY = false;
    
    void Update()
    {
        if(Input.GetMouseButton(0) && onYelkovan /*continueDraggingA == false*/) { continueDraggingY = true;}
        if (Input.GetMouseButton(0) == false) {continueDraggingY = false;}
        if (continueDraggingY)
        {
            SmousePos = Input.mousePosition;
            mousePos = Camera.main.ScreenToWorldPoint(SmousePos);
            MouseYol = new Vector2(mousePos.x - saatMerkez.position.x, mousePos.y - saatMerkez.position.y);
            angle = Vector2.Angle(forwardVector, mousePos);
            if (mousePos.x > 0) { angle = -1 * angle; }
            Debug.Log(mousePos);
            Debug.Log(saatMerkez.position);
            Debug.Log(angle);
            yelkovanOtr.localEulerAngles = new Vector3(0, 0, angle);
        }
    }

    void OnMouseOver()
    {
        if (true) { onYelkovan = true; }  
    }
    void OnMouseExit()
    {
        onYelkovan = false;
    }

}
