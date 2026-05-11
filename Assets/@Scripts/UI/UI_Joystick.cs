using UnityEngine;

public class UI_Joystick : MonoBehaviour
{
    [SerializeField] private GameObject _background;
    [SerializeField] private GameObject _cursor;

    private float _radius;
    private Vector2 _touchPos;
    private int _activeTouchId = -1;

    public bool IsActive { get; private set; }

    private static bool _isBlocked;
    public static void SetBlocked(bool blocked) => _isBlocked = blocked;

    private void Start()
    {
        RectTransform rt = _background.GetComponent<RectTransform>();
        _radius = rt.rect.height * rt.lossyScale.y / 4;
        _background.SetActive(false);
        _cursor.SetActive(false);
    }

    private void Update()
    {
        PlayerController player = GameManager.Instance.Player;
        if (_isBlocked || (player != null && player.IsLocked))
        {
            if (IsActive) Deactivate();
            return;
        }

        if (Input.touchCount > 0)
            HandleTouch();
        else
            HandleMouse();
    }

    private void HandleTouch()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began && _activeTouchId == -1)
            {
                _activeTouchId = touch.fingerId;
                Activate(touch.position);
            }
            else if (touch.fingerId == _activeTouchId)
            {
                if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                    UpdateDrag(touch.position);
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    Deactivate();
            }
        }
    }

    private void HandleMouse()
    {
        if (Input.GetMouseButtonDown(0))
            Activate(Input.mousePosition);
        else if (Input.GetMouseButton(0) && IsActive)
            UpdateDrag(Input.mousePosition);
        else if (!Input.GetMouseButton(0) && IsActive)
            Deactivate();
    }

    private void Activate(Vector2 pos)
    {
        _touchPos = pos;
        _background.SetActive(true);
        _cursor.SetActive(true);
        _background.transform.position = pos;
        _cursor.transform.position = pos;
        IsActive = true;
    }

    private void UpdateDrag(Vector2 currentPos)
    {
        Vector2 touchDir = currentPos - _touchPos;
        float moveDist = Mathf.Min(touchDir.magnitude, _radius);
        _cursor.transform.position = _touchPos + touchDir.normalized * moveDist;
        GameManager.Instance.JoystickDir = touchDir.normalized;
    }

    private void Deactivate()
    {
        _background.SetActive(false);
        _cursor.SetActive(false);
        GameManager.Instance.JoystickDir = Vector2.zero;
        IsActive = false;
        _activeTouchId = -1;
    }

    public void ForceDeactivate() => Deactivate();
}
