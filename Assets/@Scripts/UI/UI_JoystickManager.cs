using System.Collections;
using UnityEngine;

public class UI_JoystickManager : MonoBehaviour
{
    [SerializeField] private GameObject _joystickIdle;
    [SerializeField] private GameObject _uiJoystick;

    private UI_Joystick _joystick;
    private Coroutine _idleCoroutine;
    private const float IDLE_TIMEOUT = 10.0f;

    private static bool _isBlocked;
    public static void SetBlocked(bool blocked)
    {
        _isBlocked = blocked;
        UI_Joystick.SetBlocked(blocked);
    }

    private void Start()
    {
        if (_joystickIdle == null || _uiJoystick == null)
        {
            enabled = false;
            return;
        }

        _joystickIdle.SetActive(true);
        _uiJoystick.SetActive(true);

        _joystick = _uiJoystick.GetComponentInChildren<UI_Joystick>(true);
    }

    private void Update()
    {
        bool blocked = _isBlocked || (GameManager.Instance.Player != null && GameManager.Instance.Player.IsLocked);
        if (blocked)
        {
            if (_joystick != null) _joystick.ForceDeactivate();
            ShowIdle();
            return;
        }

        bool hasInput = Input.touchCount > 0 || Input.GetMouseButton(0);

        if (hasInput)
        {
            _joystickIdle.SetActive(false);
            CancelIdleTimer();
        }
        else
        {
            if (_idleCoroutine == null && !_joystickIdle.activeSelf)
                _idleCoroutine = StartCoroutine(IdleCountdown());
        }
    }

    private void ShowIdle()
    {
        CancelIdleTimer();
        _joystickIdle.SetActive(true);
    }

    private void CancelIdleTimer()
    {
        if (_idleCoroutine == null) return;
        StopCoroutine(_idleCoroutine);
        _idleCoroutine = null;
    }

    private IEnumerator IdleCountdown()
    {
        yield return new WaitForSeconds(IDLE_TIMEOUT);
        _joystickIdle.SetActive(true);
        _idleCoroutine = null;
    }
}
