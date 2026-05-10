using UnityEngine;

/// <summary>
/// Rock 오브젝트에 부착합니다.
/// 무기의 Trigger Collider가 이 Rock에 닿으면 채굴을 처리합니다.
///
/// Inspector 설정:
///   - 무기 오브젝트에 Tag = "Weapon" 설정
///   - 무기 오브젝트에 Collider (Is Trigger = true) 설정 (스크립트 불필요)
/// </summary>
[RequireComponent(typeof(Rock))]
public class WeaponInteraction : MonoBehaviour
{
    private Rock _rock;

    private void Awake()
    {
        _rock = GetComponent<Rock>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Weapon")) return;
        if (!_rock.IsAvailable) return;

        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player == null) return;

        _rock.Mine();
        player.OnRockMined();
    }
}
