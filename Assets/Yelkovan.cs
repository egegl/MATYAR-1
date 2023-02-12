using UnityEngine;
using UnityEngine.EventSystems;

public class Yelkovan : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private CanvasGroup _akrepCGroup;
    private CanvasGroup _cGroup;
    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _cGroup = GetComponent<CanvasGroup>();
        _akrepCGroup = transform.parent.GetChild(0).GetComponent<CanvasGroup>();
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        _cGroup.alpha = .6f;
        _akrepCGroup.interactable = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 10f * Time.deltaTime);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _cGroup.alpha = 1f;
        _akrepCGroup.interactable = false;
    }
}
