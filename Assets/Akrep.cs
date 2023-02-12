using UnityEngine;
using UnityEngine.EventSystems;

public class Akrep : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    //private RectTransform _rectTransform;
    private CanvasGroup _cGroup;
    private CanvasGroup _yelkovanCGroup;
    private Vector2 _clockPos;
    private Vector2 _forwardVector = new Vector2(0, 1);

    public static bool dragging;

    private void Awake()
    {
        //_rectTransform = GetComponent<RectTransform>();
        _cGroup = GetComponent<CanvasGroup>();
        _yelkovanCGroup = transform.parent.GetChild(1).GetComponent<CanvasGroup>();
        _clockPos = transform.parent.localPosition;
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        _cGroup.alpha = .6f;
        _yelkovanCGroup.interactable = false;
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
        _yelkovanCGroup.interactable = true;
    }
}
