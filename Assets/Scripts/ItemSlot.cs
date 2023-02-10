using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    private RectTransform _rectTransform;
    
    [SerializeField] private GameObject circlePrefab;

    public int NumCircles { get; set; }

    public void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    //Add circle to slot on drop
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;
        
        GameObject circle = eventData.pointerDrag;
        DragDrop dragDrop = circle.GetComponent<DragDrop>();
        Image image = circle.GetComponent<Image>();

        // don't allow more than 10 circles in slot
        if (NumCircles < 10)
        {
            dragDrop.ItemSlot = this;
            dragDrop.FirstSlotPos = _rectTransform.GetChild(0).position;

            // make circle's tag same with the slot
            circle.tag = "Circ" + tag;

            // move circle to slot, start drag cooldown and increment NumCircles
            circle.GetComponent<RectTransform>().LeanMoveLocal(_rectTransform.localPosition + _rectTransform.GetChild(NumCircles).gameObject.GetComponent<RectTransform>().localPosition, .2f).setEaseInOutQuart();
            StartCoroutine(DragCooldown(dragDrop));
            NumCircles++;

            // change color of circle according to slot
            if (CompareTag("First"))
            {
                StartCoroutine(LevelManager.Instance.ColorChange(image, Color.white, Color.green, .1f));
            }
            else
            {
                StartCoroutine(LevelManager.Instance.ColorChange(image, Color.white, Color.red, .1f));
            }

            // spawn new circle prefab on canvas at the slotted circle's start position
            SpawnCircle(LevelManager.Instance.StartLocalPos, LevelManager.Instance.StartLocalScale);
        }
        else
        {
            dragDrop.ResetCircle();
        }
    }

    // spawn new circle prefab on canvas at given position and scale
    public void SpawnCircle(Vector2 startLocalPos, Vector3 startLocalScale)
    {
        GameObject circle = Instantiate(circlePrefab, startLocalPos, Quaternion.identity);
        circle.transform.SetParent(transform.parent, false);
        circle.transform.localScale = startLocalScale;
        StartCoroutine(LevelManager.Instance.AlphaChange(circle.GetComponent<CanvasGroup>(), 0f, 1f, .3f));
        circle.SetActive(true);
    }
    
    // wait before setting inSlot to true (avoids spamming the X)
    private static IEnumerator DragCooldown(DragDrop dragDrop)
    {
        yield return new WaitForSeconds(.1f);
        dragDrop.InSlot = true;
    }
}
