using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class akrepScript : MonoBehaviour
{
    public GameObject yelkovan;

    public Transform akrepOtr;
    public Transform saatMerkez;

    public float health = 100;

    private float angle;
    Vector2 mousePos;
    Vector2 MouseYol;
    Vector2 forwardVector = new Vector2(0, 1);
    Vector2 SmousePos;

    static public bool onAkrep = false;
    public bool continueDraggingA;
    
    void Update()
    {
        if (Input.GetMouseButton(0) && onAkrep) { continueDraggingA = true; }
        if (Input.GetMouseButton(0) == false) { continueDraggingA = false; }
        if (continueDraggingA)
        {
            SmousePos = Input.mousePosition;
            mousePos = Camera.main.ScreenToWorldPoint(SmousePos);
            MouseYol = new Vector2(mousePos.x - saatMerkez.position.x, mousePos.y - saatMerkez.position.y);
            angle = Vector2.Angle(forwardVector, mousePos);
            if (mousePos.x > 0) { angle = -1 * angle; }
            Debug.Log(mousePos);
            Debug.Log(saatMerkez.position);
            Debug.Log(angle);
            akrepOtr.localEulerAngles = new Vector3(0, 0, angle);
        }
    }

    void OnMouseOver()
    {
        onAkrep = true;

    }
    void OnMouseExit()
    {
        onAkrep = false;
    }
}
