using UnityEngine;

public class UI_LinkButton : MonoBehaviour
{
    [SerializeField] private string _url;

    public void OnClick() => Application.OpenURL(_url);
}
