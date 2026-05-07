using System.Collections;
using UnityEngine;

public class UI_JoystickManager : MonoBehaviour
{
    [SerializeField] private GameObject _joystickIdle;
    [SerializeField] private GameObject _uiJoystick;

    private const float IDLE_TIMEOUT = 5.0f;
    private Coroutine _idleCoroutine;

    private void Start()
    {
        if (_joystickIdle == null || _uiJoystick == null)
        {
            Debug.LogError("[UI_JoystickManager] Inspector에서 _joystickIdle, _uiJoystick을 모두 연결해주세요.", this);
            enabled = false;
            return;
        }

        _joystickIdle.SetActive(true);
        _uiJoystick.SetActive(false);
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            if (!_uiJoystick.activeSelf)
                ActivateJoystick();

            CancelIdleTimer();
        }
        else
        {
            if (_uiJoystick.activeSelf && _idleCoroutine == null)
                _idleCoroutine = StartCoroutine(IdleCountdown());
        }
    }

    private void ActivateJoystick()
    {
        _joystickIdle.SetActive(false);
        _uiJoystick.SetActive(true);
    }

    private void DeactivateJoystick()
    {
        _uiJoystick.SetActive(false);
        _joystickIdle.SetActive(true);
    }

    private void CancelIdleTimer()
    {
        if (_idleCoroutine == null)
            return;

        StopCoroutine(_idleCoroutine);
        _idleCoroutine = null;
    }

    private IEnumerator IdleCountdown()
    {
        yield return new WaitForSeconds(IDLE_TIMEOUT);
        DeactivateJoystick();
        _idleCoroutine = null;
    }
}
