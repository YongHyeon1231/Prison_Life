using UnityEngine;

public class UI_Handler : MonoBehaviour
{
    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
}
