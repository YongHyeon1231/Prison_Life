using UnityEngine;

public class UI_SoundToggle : MonoBehaviour
{
    [SerializeField] private GameObject _soundOnObject;
    [SerializeField] private GameObject _soundOffObject;

    private void Start()
    {
        Refresh();
    }

    public void OnClick()
    {
        bool muted = GameManager.Instance.Sound.IsMuted;
        GameManager.Instance.Sound.SetMute(!muted);
        Refresh();
    }

    private void Refresh()
    {
        bool muted = GameManager.Instance.Sound.IsMuted;
        if (_soundOnObject  != null) _soundOnObject.SetActive(!muted);
        if (_soundOffObject != null) _soundOffObject.SetActive(muted);
    }
}
