using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    private RectTransform _rectTransform;
    private int _index;

    [SerializeField] private GameObject circlePrefab;

    public int NumCircles { get; set; }

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();

        if (name.Equals("FirstSlot")) _index = 1;
        else _index = 2;
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
            // update circle's slot
            dragDrop.ChangeSlot(this, _rectTransform.GetChild(0).position, _index);

            // move circle to slot, start drag cooldown and increment NumCircles
            circle.GetComponent<RectTransform>().LeanMoveLocal(_rectTransform.localPosition + _rectTransform.GetChild(NumCircles).gameObject.GetComponent<RectTransform>().localPosition, .2f).setEaseInOutQuart();
            NumCircles++;

            // change color of circle according to slot
            if (_index == 1) StartCoroutine(LevelManager.Instance.ColorChange(image, Color.white, Color.green, .1f));
            else StartCoroutine(LevelManager.Instance.ColorChange(image, Color.white, Color.red, .1f));

            // spawn new circle prefab on canvas at the slotted circle's start position
            SpawnCircle();
        }
        else dragDrop.ResetCircle();
        }

    // spawn new circle prefab on canvas at given position and scale
    public void SpawnCircle()
    {
        int randZ = Random.Range(0, 360);
        GameObject circle = Instantiate(circlePrefab, transform.parent, false);
        circle.transform.Rotate(0f, 0f, randZ);
        circle.transform.GetChild(0).Rotate(0f, 0f, -randZ);
        StartCoroutine(LevelManager.Instance.AlphaChange(circle.GetComponent<CanvasGroup>(), 0f, 1f, .3f));
        circle.SetActive(true);
    }
}
