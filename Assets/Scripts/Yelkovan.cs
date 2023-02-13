using UnityEngine;
using UnityEngine.EventSystems;

public class Yelkovan : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private CanvasGroup _cGroup;
    private Transform _pivot;
    [SerializeField] private CanvasGroup akrepCGroup;
    [SerializeField] private Transform akrepPivot;

    public static Yelkovan Instance { get; private set; }

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
        akrepCGroup.interactable = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        
        int i = 0;
        Vector3 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - _pivot.position;
        direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        _pivot.rotation = Quaternion.Euler(0f, 0f, angle - 90);
        //akrepPivot.rotation = Quaternion.Euler(0f, 0f, akrepPivot.rotation.z + (angle / 12)); fix later
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _cGroup.alpha = 1f;
        akrepCGroup.interactable = false;

        UpdateMins();
        Akrep.Instance.UpdateHours();
    }

    // update minutes on the digital clock
    public void UpdateMins()
    {
        Level2Manager.Instance.UpdateMins(Minute(360 - _pivot.rotation.eulerAngles.z));
    }

    // calculate minute from rotation
    private int Minute(float rotation)
    {
        int ans = Mathf.RoundToInt(rotation);
        return ans / 6;
    }
}
