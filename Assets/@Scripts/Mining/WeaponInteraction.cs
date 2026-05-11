using UnityEngine;

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
