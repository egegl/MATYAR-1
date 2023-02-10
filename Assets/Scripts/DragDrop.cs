using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDrop : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Canvas _canvas;
    private Image _image;
    private CanvasGroup _canvasGroup;
    private RectTransform _rectTransform;
    private GameObject _cross;
    private Vector2 _currPos;
    private Vector2 _removedPos;

    public ItemSlot ItemSlot { private get; set; }
    public Vector2 FirstSlotPos { private get; set; }
    public bool InSlot { private get; set; }

    private void Awake()
    {
        _canvas = GetComponentInParent<Canvas>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _rectTransform = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
        _cross = transform.GetChild(0).gameObject;
    }

    // cross over the circle on mouse hover if in slot
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (InSlot)
        {
            _cross.SetActive(true);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (InSlot)
        {
            _cross.SetActive(false);
        }
    }
    
    // remove circle from slot on left click if in slot + update numCircles + set inSlot to false
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!InSlot) return;
        if (eventData.button != PointerEventData.InputButton.Left) return;
        
        // decrement the number of circles in the slot
        ItemSlot.NumCircles--;

        // get current tag and reset circle
        string tempTag = tag;
        ResetCircle();
                
        // move back rest of the circles in slot if the removed circle is in the same slot
        foreach (GameObject circle in GameObject.FindGameObjectsWithTag(tempTag))
        {
            DragDrop circleDd = circle.GetComponent<DragDrop>();
            circleDd.MoveBack(_removedPos);
        }
    }

    // reset circle to original state, then destroy it if number of circles outside of slots (including itself) is greater than 1
    public void ResetCircle()
    {
        _cross.SetActive(false);
        tag = "Circ";
        InSlot = false;
        _removedPos = _rectTransform.position;
        _rectTransform.LeanMoveLocal(LevelManager.Instance.StartLocalPos, .4f).setEaseInOutQuart();
        StartCoroutine(LevelManager.Instance.ColorChange(_image, _image.color, Color.white, .2f));
        
        if(GameObject.FindGameObjectsWithTag(tag).Length > 1)
        {
            Destroy(gameObject, .4f);
        }
    }

    // rearrange circles in slot
    private void MoveBack(Vector2 otherRemovedPos)
    {
        _currPos = _rectTransform.position; // not local position
        Vector2 currLocalPos = _rectTransform.localPosition;
        
        // if removed circle is below current circle OR if removed circle is on the same row but to the right of current circle, don't move
        if (_currPos.y > otherRemovedPos.y || ((int)_currPos.y == (int)otherRemovedPos.y && _currPos.x < otherRemovedPos.x))
        {
            return;
        }

        // if circle is on the right side of the slot, move it to the left
        else if (_currPos.x > FirstSlotPos.x)
        {
            _rectTransform.LeanMoveLocalX(currLocalPos.x - 60, .14f); // .14 is roughly .2 / sqrt(2)
        }

        // if circle is on the left side of the slot, move it to the right
        else
        {
            if(_currPos.y < FirstSlotPos.y)
            {
                _rectTransform.LeanMoveLocal(new Vector2(currLocalPos.x + 60, currLocalPos.y + 60), .2f); // roughly .14 * sqrt(2)
            }
        }
    }

    // initialize mouse drag, spawn new circle to start position if needed
    public void OnBeginDrag(PointerEventData eventData)
    {
        // allow to interact with the slot below and make translucent
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.alpha = .6f;
    }

    // move circle during mouse drag
    public void OnDrag(PointerEventData eventData)
    {
        if(!InSlot)
        {
            _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
        }
    }

    // end mouse drag
    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.alpha = 1;
        
        // reset circle if it's not dropped on a slot
        if(ItemSlot == null)
        {
            ResetCircle();
        }
    }
}
