using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class Yelkovan : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private CanvasGroup _cGroup;
    private Transform _pivot;
    private float _z1;
    private float _z2;
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
        // set initial rotation
        _z1 = 360 - _pivot.eulerAngles.z;
        if (_z1 == 360) _z1 = 0;

        _cGroup.alpha = .6f;
        akrepCGroup.interactable = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // rotate yelkovan
        Vector3 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - _pivot.position;
        direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _pivot.eulerAngles = new Vector3(0f, 0f, angle - 90f);
        //_pivot.Rotate(0f, 0f, angle - _pivot.eulerAngles.z - 90);

        // get rotation difference
        _z2 = 360f - _pivot.eulerAngles.z;
        float zDiff = _z2 - _z1;

        // calculate the relative rotation of akrep
        float akrepRot = _z2 / 12f;
        akrepPivot.rotation = Quaternion.Euler(0f, 0f, -akrepRot);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _cGroup.alpha = 1f;
        akrepCGroup.interactable = true; 

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
