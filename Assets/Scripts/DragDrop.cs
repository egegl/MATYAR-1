using System;
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
    private bool _inSlot;
    private int _slotIndex;
    private ItemSlot _itemSlot;
    private Vector2 _firstSlotPos;
    
    private static readonly List<GameObject> FirstCircList = new List<GameObject>();
    private static readonly List<GameObject> SecondCircList = new List<GameObject>();
    private static readonly Vector3 StartLocalPos  = new(-185f, 27.2f, 0f);
    
    public static GameObject SpawnCircle { get; private set; }
    public static Dictionary<int, List<GameObject>> CircDict = new Dictionary<int, List<GameObject>> 
    {
        [1] = FirstCircList,
        [2] = SecondCircList
    };

    private void Awake()
    {
        _canvas = GetComponentInParent<Canvas>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _rectTransform = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
        _cross = transform.GetChild(0).gameObject;
        
        SpawnCircle = gameObject;
    }

    private void Start()
    {
        // fade in on start
        _canvasGroup.LeanAlpha(1f, .3f);
    }

    // cross over the circle on mouse hover if in slot
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_inSlot) _cross.SetActive(true);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (_inSlot) _cross.SetActive(false);
    }
    
    // remove circle from slot on left click if in slot + update numCircles + set inSlot to false
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_inSlot) return;
        if (eventData.button != PointerEventData.InputButton.Left) return;
        
        // decrement the number of circles in the slot
        _itemSlot.NumCircles--;

        // get slot index and reset circle
        int slotIndex = _slotIndex;
        ResetCircle();
                
        // move back rest of the circles in slot if the removed circle is in the same slot
        foreach (GameObject circle in CircDict[slotIndex])
        {
            circle.GetComponent<DragDrop>().MoveBack(_removedPos);
        }
    }

    // reset circle to original state, then destroy it
    public void ResetCircle()
    {
        _cross.SetActive(false);
        _inSlot = false;
        _removedPos = _rectTransform.position;
        _rectTransform.LeanMoveLocal(StartLocalPos, .4f).setEaseInOutQuart();
        StartCoroutine(GameManager.Instance.ColorChange(_image, _image.color, Color.white, .2f));

        if (_slotIndex > 0)
        {
            CircDict[_slotIndex].Remove(gameObject);
            Destroy(gameObject, .4f);
        }
    }

    // reset circle lists
    public static void ResetLists()
    {
        SecondCircList.Clear();
        FirstCircList.Clear();
    }

    // change slot of the circle
    public void ChangeSlot(ItemSlot itemSlot, Vector2 firstSlotPos, int slotIndex)
    {
        _itemSlot = itemSlot;
        _firstSlotPos = firstSlotPos;
        _slotIndex = slotIndex;

        CircDict[_slotIndex].Add(gameObject);
        StartCoroutine(ActivateInSlot());
    }
    
    // wait before setting inSlot to true (avoids spamming the X)
    private IEnumerator ActivateInSlot()
    {
        yield return new WaitForSeconds(.1f);
        yield return _inSlot = true;
    }

    // rearrange circles in slot
    private void MoveBack(Vector2 otherRemovedPos)
    {
        _currPos = _rectTransform.position; // not local position
        Vector2 currLocalPos = _rectTransform.localPosition;
        
        // if removed circle is below current circle OR if removed circle is on the same row but to the right of current circle, don't move
        if (_currPos.y > otherRemovedPos.y || (Mathf.Approximately(_currPos.y, otherRemovedPos.y) && Mathf.Approximately(_currPos.x, _firstSlotPos.x))) return;
        
        // same y level but move
        if (Mathf.Approximately(_removedPos.y, _firstSlotPos.y) && Mathf.Approximately(_currPos.y, _firstSlotPos.y))
        {
            _rectTransform.LeanMoveLocalX(currLocalPos.x - 60, .14f); // .14 is roughly .2 / sqrt(2)
        }
        // below
        else
        {
            // left
            if (Mathf.Approximately(_currPos.x, _firstSlotPos.x))
            {
                _rectTransform.LeanMoveLocal(new Vector2(currLocalPos.x + 60, currLocalPos.y + 60), .2f);
            }
            // right
            else
            {
                _rectTransform.LeanMoveLocalX(currLocalPos.x - 60, .14f); // .14 is roughly .2 / sqrt(2)
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
        if (_inSlot) return;
        _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }

    // end mouse drag
    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.alpha = 1;
        
        // reset circle if it's not dropped on a slot
        if(_itemSlot == null) _rectTransform.LeanMoveLocal(StartLocalPos, .4f).setEaseInOutQuart();
    }
}
