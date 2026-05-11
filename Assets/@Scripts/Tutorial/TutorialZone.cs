using UnityEngine;

public class TutorialZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            TutorialGuide.Instance?.CompleteCurrentStep();
    }
}
