using UnityEngine;
using UnityEngine.EventSystems;

public class Akrep : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private float _z1;
    private float _yelkovanFb;
    private CanvasGroup _cGroup;
    private Transform _pivot;
    [SerializeField] private Transform yelkovanPivot;

    public int Hr { get; set; }
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
        _z1 = _pivot.rotation.eulerAngles.z;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - _pivot.position;
        direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        AkrepDrag(angle - 90);
    }

    public void AkrepDrag(float angle)
    {
        // rotate akrep
        _pivot.rotation = Quaternion.Euler(0f, 0f, angle);
        
        // calculate feedback to yelkovan
        float zDiff = angle - _z1;
        _yelkovanFb = zDiff * 12;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _cGroup.alpha = 1f;

        // update hours
        Hr = Hour(_pivot.eulerAngles.z);

        // feedback to minutes
        Yelkovan.Instance.Feedback(_yelkovanFb);
    }

    public void Feedback(int curr, int prev)
    {
        // calculate ratios
        float currRatio = curr / -2;
        float prevRatio = prev / -2;

        // calculate difference of ratios and new angle
        float ratio = currRatio - prevRatio;
        float newAngle = _pivot.eulerAngles.z + ratio;

        // update hours
        Hr = Hour(_pivot.eulerAngles.z);

        // rotate self
        _pivot.LeanRotateZ(newAngle, .3f).setEaseOutBack();
    }

    // reset rotation of pivot
    public void Reset()
    {
        _pivot.rotation = Quaternion.identity;
        Hr = 0;
    }

    // calculate hour from rotation
    private int Hour(float rotation)
    {
        if (rotation < .1f && rotation > -.1f) rotation = 0;
        int ans = (int)(360 - rotation);
        ans /= 30;
        if (ans == 12) ans = 0;
        return ans;
    }
}
