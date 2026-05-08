using UnityEngine;

/// <summary>
/// 무기 오브젝트에 부착합니다.
/// Box Collider(Is Trigger = true)가 Rock 태그의 오브젝트와 접촉하면
/// 부모 계층에서 PlayerController 를 찾아 채굴 상태를 알립니다.
///
/// Rock GameObject 에는 반드시 태그 "Rock" 이 설정되어 있어야 합니다.
/// Rock GameObject 에는 Rock 컴포넌트(또는 IDamageable 등)를 붙여 파괴 로직을 처리합니다.
/// </summary>
[RequireComponent(typeof(Collider))]
public class WeaponCollider : MonoBehaviour
{
    private PlayerController _player;

    private void Awake()
    {
        _player = GetComponentInParent<PlayerController>();

        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Rock")) return;
        if (_player == null) return;
        // Rock 을 채굴(파괴)하고 플레이어에게 채굴 상태를 알립니다.
        Rock rock = other.GetComponent<Rock>();
        if (rock != null && rock.IsAvailable)
        {
            rock.Mine();                  // Rock 비활성화 + 풀 반환
            _player.OnRockMined();        // 플레이어 상태 갱신 + RockPile 추가
        }
    }

}