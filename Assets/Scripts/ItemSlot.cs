using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    private RectTransform m_rectTransform;

    public int numCircles;
    public GameObject circlePrefab;

    public void Awake()
    {
        m_rectTransform = GetComponent<RectTransform>();
        numCircles = 0;
    }

    //Add circle to slot on drop
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            GameObject m_circle = eventData.pointerDrag;
            DragDrop m_dragDrop = m_circle.GetComponent<DragDrop>();
            Image m_image = m_circle.GetComponent<Image>();

            // don't allow more than 10 circles in slot
            if (numCircles < 10)
            {
                m_dragDrop.ItemSlot = this;
                m_dragDrop.FirstSlotPos = m_rectTransform.GetChild(0).position;

                // make circle's tag same with the slot
                m_circle.tag = "Circ" + tag;

                // move circle to slot, start drag cooldown and increment numCircles
                m_circle.GetComponent<RectTransform>().LeanMoveLocal(m_rectTransform.localPosition + m_rectTransform.GetChild(numCircles).gameObject.GetComponent<RectTransform>().localPosition, .2f).setEaseInOutQuart();
                StartCoroutine(DragCooldown(m_dragDrop));
                numCircles++;

                // change color of circle according to slot
                if (CompareTag("First"))
                {
                    StartCoroutine(LevelManager.Instance.ColorChange(m_image, Color.white, Color.green, .1f));
                }
                else
                {
                    StartCoroutine(LevelManager.Instance.ColorChange(m_image, Color.white, Color.red, .1f));
                }

                // spawn new circle prefab on canvas at the slotted circle's start position
                SpawnCircle(LevelManager.Instance.StartLocalPos, LevelManager.Instance.StartLocalScale);
            }
            else
            {
                m_dragDrop.ResetCircle();
            }
        }
    }

    // spawn new circle prefab on canvas at given position and scale
    public void SpawnCircle(Vector2 startPos, Vector3 startLocalScale)
    {
        GameObject m_circ = Instantiate(circlePrefab, startPos, Quaternion.identity);
        m_circ.transform.SetParent(transform.parent, false);
        m_circ.transform.localScale = startLocalScale;
        StartCoroutine(LevelManager.Instance.AlphaChange(m_circ.GetComponent<CanvasGroup>(), 0f, 1f, .3f));
        m_circ.SetActive(true);
    }
    
    // wait before setting inSlot to true (avoids spamming the X)
    private IEnumerator DragCooldown(DragDrop dragDrop)
    {
        yield return new WaitForSeconds(.15f);
        dragDrop.InSlot = true;
    }
}
