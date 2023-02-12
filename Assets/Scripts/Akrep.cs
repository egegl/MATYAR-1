using UnityEngine;
using UnityEngine.EventSystems;

public class Akrep : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private CanvasGroup _cGroup;
    private Transform _pivot;
    private float _prevZ;
    [SerializeField] private CanvasGroup yelkovanCGroup;
    [SerializeField] private Transform yelkovanPivot;

    public static Akrep Instance { get; private set; }

    private void Awake()
    {
        // singleton
        Instance = this;

        _cGroup = GetComponent<CanvasGroup>();
        _pivot = transform.parent.transform;
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        _cGroup.alpha = .6f;
        yelkovanCGroup.interactable = false;
        _prevZ= _pivot.eulerAngles.z;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - _pivot.position;
        direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _pivot.rotation = Quaternion.Euler(0f, 0f, angle - 90);
        yelkovanPivot.rotation = Quaternion.Euler(0f, 0f, yelkovanPivot.rotation.z + angle * 12);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _cGroup.alpha = 1f;
        
        UpdateHours();
        Yelkovan.Instance.UpdateMins();
    }

    // update hours on the digital clock
    public void UpdateHours()
    {
        Level2Manager.Instance.UpdateHours(Hour(360 - _pivot.rotation.eulerAngles.z));
    }

    // calculate hour from rotation
    private int Hour(float rotation)
    {
        int ans = Mathf.RoundToInt(rotation);
        return ans / 30;
    }
}
