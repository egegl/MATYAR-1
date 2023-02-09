using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDrop : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Canvas m_canvas;
    private Image m_image;
    private CanvasGroup m_canvasGroup;
    private RectTransform m_rectTransform;
    private bool m_inSlot;
    private GameObject m_cross;
    private Vector2 m_currPos;
    private Vector2 m_removedPos;
    private Vector2 m_firstSlotPos;
    private ItemSlot m_itemSlot;
    
    private static Vector3 StartLocalPos = new(-185f, 40f, 0f);
    private static Vector3 StartLocalScale = new(1.1f, 1.1f, 1.1f);

    private void Awake()
    {
        m_inSlot = false;
        m_canvas = GetComponentInParent<Canvas>();
        m_canvasGroup = GetComponent<CanvasGroup>();
        m_rectTransform = GetComponent<RectTransform>();
        m_image = GetComponent<Image>();
        m_cross = transform.GetChild(0).gameObject;
    }
    
    // m_inSlot getter setter
    public bool GetInSlot()
    {
        return m_inSlot;
    }
    public void SetInSlot(bool newInSlot)
    {
        m_inSlot = newInSlot;
    }

    // m_firstSlotPos getter setter
    public Vector2 GetFirstSlotPos()
    {
        return m_firstSlotPos;
    }
    public void SetFirstSlotPos(Vector2 newFirstSlotPos)
    {
        m_firstSlotPos = newFirstSlotPos;
    }

    //m_itemSlot getter setter
    public ItemSlot GetItemSlot()
    {
        return m_itemSlot;
    }
    public void SetItemSlot(ItemSlot newItemSlot)
    {
        m_itemSlot = newItemSlot;
    }

    // StartLocalPos getter setter
    public static Vector2 GetStartLocalPos()
    {
        return StartLocalPos;
    }
    public static void SetStartLocalPos(Vector2 newStartLocalPos)
    {
        StartLocalPos = newStartLocalPos;
    }

    // StartLocalScale getter setter
    public static Vector3 GetStartLocalScale()
    {
        return StartLocalScale;
    }
    public static void SetStartLocalScale(Vector3 newStartLocalScale)
    {
        StartLocalScale = newStartLocalScale;
    }

    // cross over the circle on mouse hover if in slot
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(m_inSlot)
        {
            m_cross.SetActive(true);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (m_inSlot)
        {
            m_cross.SetActive(false);
        }
    }
    
    // remove circle from slot on left click if in slot + update numCircles + set inSlot to false
    public void OnPointerClick(PointerEventData eventData)
    {
        if(m_inSlot)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                // decrement the number of circles in the slot
                m_itemSlot.numCircles--;

                // get current tag and reset circle
                string tempTag = tag;
                ResetCircle();
                
                // move back rest of the circles in slot if the removed circle is in the same slot
                foreach (GameObject circle in GameObject.FindGameObjectsWithTag(tempTag))
                {
                    DragDrop circleDd = circle.GetComponent<DragDrop>();
                    circleDd.MoveBack(m_removedPos);
                }
            }
        }
    }

    // reset circle to original state, then destroy it if number of circles outside of slots (including itself) is greater than 1
    public void ResetCircle()
    {
        m_cross.SetActive(false);
        tag = "Circ";
        m_removedPos = m_rectTransform.position;
        m_rectTransform.LeanMoveLocal(StartLocalPos, .4f).setEaseInOutQuart();
        StartCoroutine(ColorChange(m_image.color, Color.white, .2f));
        
        if(GameObject.FindGameObjectsWithTag(tag).Length > 1)
        {
            Destroy(gameObject, .4f);
        }
    }

    // rearrange circles in slot
    public void MoveBack(Vector2 otherRemovedPos)
    {
        m_currPos = m_rectTransform.position; // not local position
        Vector2 m_currLocalPos = m_rectTransform.localPosition;
        
        // if removed circle is below current circle OR if removed circle is on the same row but to the right of current circle, don't move
        if (m_currPos.y > otherRemovedPos.y || (m_currPos.y == otherRemovedPos.y && m_currPos.x < otherRemovedPos.x))
        {
            return;
        }

        // if circle is on the right side of the slot, move it to the left
        else if (m_currPos.x > m_firstSlotPos.x)
        {
            m_rectTransform.LeanMoveLocalX(m_currLocalPos.x - 60, .14f); // .14 is roughly .2 / sqrt(2)
        }

        // if circle is on the left side of the slot, move it to the right
        else
        {
            if(m_currPos.y < m_firstSlotPos.y)
            {
                m_rectTransform.LeanMoveLocal(new Vector2(m_currLocalPos.x + 60, m_currLocalPos.y + 60), .2f); // roughly .14 * sqrt(2)
            }
        }
    }

    // initialize mouse drag, spawn new circle to start position if needed
    public void OnBeginDrag(PointerEventData eventData)
    {
        // allow to interact with the slot below and make translucent
        m_canvasGroup.blocksRaycasts = false;
        m_canvasGroup.alpha = .6f;
    }

    // move circle during mouse drag
    public void OnDrag(PointerEventData eventData)
    {
        if(!m_inSlot)
        {
            m_rectTransform.anchoredPosition += eventData.delta / m_canvas.scaleFactor;
        }
    }

    // end mouse drag
    public void OnEndDrag(PointerEventData eventData)
    {
        m_canvasGroup.blocksRaycasts = true;
        m_canvasGroup.alpha = 1;
        
        // reset circle if it's not dropped on a slot
        if(m_itemSlot == null)
        {
            ResetCircle();
        }
    }

    // smooth color change
    public IEnumerator ColorChange(Color from, Color to, float duration)
    {
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            m_image.color = Color.Lerp(from, to, (t / duration));
            yield return null;
        }
    }
}
