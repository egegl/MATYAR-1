using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDrop : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Canvas canvas;
    private Image image;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private GameObject x;
    private Vector2 currPos;
    private Vector2 removedPos;

    public bool inSlot;
    public Vector2 startPos;
    public Vector2 firstSlot;
    public Vector3 startLocalScale;
    public ItemSlot itemSlot;

    private void Awake()
    {
        inSlot = false;
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        startPos = rectTransform.localPosition;
        startLocalScale = rectTransform.localScale;
        x = this.transform.GetChild(0).gameObject;
    }

    // Red X on mouse hover if in slot
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(inSlot)
        {
            x.SetActive(true);
        }
    }
    public void OnPointerExit(PointerEventData eventData) => x.SetActive(false);

    // remove circle from slot on left click if in slot + update numCircles + set inSlot to false
    public void OnPointerClick(PointerEventData eventData)
    {
        if(inSlot)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                // decrement the number of circles in the slot
                itemSlot.numCircles--;

                // get current tag and reset circle
                string tempTag = tag;
                ResetCircle();
                
                // move back rest of the circles in slot if the removed circle is in the same slot
                foreach (GameObject circle in GameObject.FindGameObjectsWithTag(tempTag))
                {
                    DragDrop circleDd = circle.GetComponent<DragDrop>();
                    circleDd.MoveBack(removedPos);
                }
            }
        }
    }

    // reset circle to original state, then destroy it if number of circles outside of slots (including itself) is greater than 1
    public void ResetCircle()
    {
        x.SetActive(false);
        tag = "Circ";
        removedPos = rectTransform.position;
        rectTransform.LeanMoveLocal(startPos, .4f).setEaseInOutQuart();
        inSlot = false;
        StartCoroutine(ColorChange(image.color, Color.white, .2f));

        if(GameObject.FindGameObjectsWithTag(tag).Length > 1)
        {
            Destroy(gameObject, .4f);
        }
    }

    // rearrange circles in slot
    public void MoveBack(Vector2 otherRemovedPos)
    {
        currPos = rectTransform.position; // not local position
        Vector2 currPosLocal = rectTransform.localPosition;
        
        // if removed circle is below current circle OR if removed circle is on the same row but to the right of current circle, don't move
        if (currPos.y > otherRemovedPos.y || (currPos.y == otherRemovedPos.y && currPos.x < otherRemovedPos.x))
        {
            return;
        }

        // if circle is on the right side of the slot, move it to the left
        else if (currPos.x > firstSlot.x)
        {
            rectTransform.LeanMoveLocal(new Vector2(currPosLocal.x - 60, currPosLocal.y), .14f); // .14 is roughly .2 / sqrt(2)
        }

        // if circle is on the left side of the slot, move it to the right
        else
        {
            if(currPos.y < firstSlot.y)
            {
                rectTransform.LeanMoveLocal(new Vector2(currPosLocal.x + 60, currPosLocal.y + 60), .2f); // roughly .14 * sqrt(2)
            }
        }
    }

    // initialize mouse drag, spawn new circle to start position if needed
    public void OnBeginDrag(PointerEventData eventData)
    {
        // allow to interact with the slot below and make translucent
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = .6f;
    }

    // move circle during mouse drag
    public void OnDrag(PointerEventData eventData)
    {
        if(!inSlot)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    // end mouse drag
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1;
        
        // reset circle if it's not dropped on a slot
        if(itemSlot == null)
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
            image.color = Color.Lerp(from, to, (t / duration));
            yield return null;
        }
    }
}
