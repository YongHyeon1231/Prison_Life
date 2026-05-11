using UnityEngine;

public class JoystickBlocker : MonoBehaviour
{
    private void OnEnable()  => UI_JoystickManager.SetBlocked(true);
    private void OnDisable() => UI_JoystickManager.SetBlocked(false);
}
