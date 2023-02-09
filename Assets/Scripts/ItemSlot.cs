using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    private RectTransform rectTransform;

    public int numCircles;
    public GameObject circlePrefab;

    public void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        numCircles = 0;
    }

    //Add circle to slot on drop
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            DragDrop dragDrop = eventData.pointerDrag.GetComponent<DragDrop>();
            if (numCircles < 10)
            {
                dragDrop.SetItemSlot(this);
                dragDrop.SetFirstSlotPos(rectTransform.GetChild(0).position);

                // make circle's tag same with the slot
                eventData.pointerDrag.tag = "Circ" + tag;

                // move circle to slot, start drag cooldown and increment numCircles
                eventData.pointerDrag.GetComponent<RectTransform>().LeanMoveLocal(rectTransform.localPosition + rectTransform.GetChild(numCircles).gameObject.GetComponent<RectTransform>().localPosition, .2f).setEaseInOutQuart();                StartCoroutine(DragCooldown(dragDrop));
                numCircles++;

                // change color of circle according to slot
                if (CompareTag("First"))
                {
                    StartCoroutine(dragDrop.ColorChange(Color.white, Color.green, .1f));
                }
                else
                {
                    StartCoroutine(dragDrop.ColorChange(Color.white, Color.red, .1f));
                }

                // spawn new circle prefab on canvas at the slotted circle's start position
                SpawnCircle(DragDrop.GetStartLocalPos(), DragDrop.GetStartLocalScale());
            }

            // don't allow more than 10 circles in slot
            else
            {
                dragDrop.ResetCircle();
            }
        }
    }

    // spawn new circle prefab on canvas at given position and scale
    public void SpawnCircle(Vector2 startPos, Vector3 startLocalScale)
    {
        GameObject newCirc = Instantiate(circlePrefab, startPos, Quaternion.identity);
        newCirc.transform.SetParent(transform.parent, false);
        newCirc.transform.localScale = startLocalScale;
        StartCoroutine(AlphaChange(newCirc.GetComponent<CanvasGroup>(), 0, 1, .3f));
        newCirc.SetActive(true);
    }
    
    // wait for .1 seconds before setting inSlot to true (avoids spamming the X)
    private IEnumerator DragCooldown(DragDrop dragDrop)
    {
        yield return new WaitForSeconds(.1f);
        dragDrop.SetInSlot(true);
    }

    // change alpha of circle smoothly (fades in & out)
    private IEnumerator AlphaChange(CanvasGroup canvasGroup, float from, float to, float duration)
    {
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }
    }
}