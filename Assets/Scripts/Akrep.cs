using UnityEngine;
using UnityEngine.EventSystems;

public class Akrep : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private CanvasGroup _cGroup;
    private Transform _pivot;
    [SerializeField] private CanvasGroup yelkovanCGroup;

    private void Awake()
    {
        _cGroup = GetComponent<CanvasGroup>();
        _pivot = transform.parent.transform;
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        _cGroup.alpha = .6f;
        yelkovanCGroup.interactable = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - _pivot.position;
        direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _pivot.rotation = Quaternion.Euler(0f, 0f, angle - 90);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _cGroup.alpha = 1f;
        yelkovanCGroup.interactable = true;
    }
}
