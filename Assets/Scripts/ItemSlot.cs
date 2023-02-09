using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    private RectTransform m_rectTransform;
    
    [SerializeField] private GameObject m_circlePrefab;

    public int NumCircles { get; set; } = 0;

    public void Awake()
    {
        m_rectTransform = GetComponent<RectTransform>();
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
            if (NumCircles < 10)
            {
                m_dragDrop.ItemSlot = this;
                m_dragDrop.FirstSlotPos = m_rectTransform.GetChild(0).position;

                // make circle's tag same with the slot
                m_circle.tag = "Circ" + tag;

                // move circle to slot, start drag cooldown and increment NumCircles
                m_circle.GetComponent<RectTransform>().LeanMoveLocal(m_rectTransform.localPosition + m_rectTransform.GetChild(NumCircles).gameObject.GetComponent<RectTransform>().localPosition, .2f).setEaseInOutQuart();
                StartCoroutine(DragCooldown(m_dragDrop));
                NumCircles++;

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
    public void SpawnCircle(Vector2 startLocalPos, Vector3 startLocalScale)
    {
        GameObject m_circle = Instantiate(m_circlePrefab, startLocalPos, Quaternion.identity);
        m_circle.transform.SetParent(transform.parent, false);
        m_circle.transform.localScale = startLocalScale;
        StartCoroutine(LevelManager.Instance.AlphaChange(m_circle.GetComponent<CanvasGroup>(), 0f, 1f, .3f));
        m_circle.SetActive(true);
    }
    
    // wait before setting inSlot to true (avoids spamming the X)
    private IEnumerator DragCooldown(DragDrop dragDrop)
    {
        yield return new WaitForSeconds(.15f);
        dragDrop.InSlot = true;
    }
}
