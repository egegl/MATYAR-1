using UnityEngine;
using UnityEngine.EventSystems;

public class Yelkovan : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private float _z1;
    private CanvasGroup _cGroup;
    private Transform _pivot;
    [SerializeField] private Transform akrepPivot;

    public int Min { get; set; }
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
        _z1 = _pivot.rotation.eulerAngles.z;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // calculate angle
        Vector3 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - _pivot.position;
        direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // rotate self
        _pivot.rotation = Quaternion.Euler(0f, 0f, angle - 90);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _cGroup.alpha = 1f;

        // update minutes
        Min = Minute(_pivot.eulerAngles.z);

        // feedback to hours
        Akrep.Instance.Feedback(Min, Minute(_z1));
    }

    public void Feedback(float fbAngle)
    {
        // adjust feedback angle
        while (fbAngle < -360) fbAngle += 360;

        // calculate new angle
        float newAngle = _pivot.eulerAngles.z + fbAngle;

        // update minutes
        Min = Minute(newAngle);

        // rotate self
        _pivot.LeanRotateZ(newAngle, .4f).setEaseOutBack();
    }

    // reset rotation of pivot
    public void Reset()
    {
        _pivot.LeanRotateZ(0, .3f).setEaseOutBack();
        Min = 0;
    }

    // calculate minute from rotation
    private int Minute(float angle)
    {
        int ans = Mathf.RoundToInt(360 - angle + .7f);
        ans /= 6;
        if (ans == 60) ans = 0;
        return ans;
    }
}
