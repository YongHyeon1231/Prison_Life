using UnityEngine;

/// <summary>
/// 무기 오브젝트에 부착합니다.
/// 채굴 감지 로직은 Rock의 WeaponInteraction.cs 로 이전됐습니다.
/// 이 컴포넌트는 무기 Collider를 Trigger로 강제하는 역할만 담당합니다.
///
/// Inspector 설정:
///   - Tag = "Weapon"
///   - Collider (Is Trigger = true)
/// </summary>
[RequireComponent(typeof(Collider))]
public class WeaponCollider : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }
}
