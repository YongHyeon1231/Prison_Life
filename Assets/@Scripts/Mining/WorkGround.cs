using UnityEngine;

/// <summary>
/// WorkGround(채굴 지형) 오브젝트에 부착합니다.
/// Box Collider(Is Trigger = true)로 플레이어 진입/이탈을 감지합니다.
///
/// Inspector 설정:
///   - Layer : WorkGround
///   - Box Collider, Is Trigger = true
/// </summary>
public class WorkGround : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;
        
        player.EnterWorkGround();
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        player.ExitWorkGround();
    }
}