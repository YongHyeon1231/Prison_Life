using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TutorialStep
{
    public TutorialZone zone;
    public Transform    arrowAnchor;
}

public class TutorialGuide : MonoBehaviour
{
    public static TutorialGuide Instance { get; private set; }

    [Header("Steps")]
    [SerializeField] private List<TutorialStep> _steps;

    [Header("Arrow")]
    [SerializeField] private GameObject _arrowPrefab;

    [Header("Compass")]
    [SerializeField] private Transform _compass;
    [SerializeField] private float     _compassShowDistance = 5f;

    private int        _currentIndex  = -1;
    private GameObject _arrowInstance;
    private Animator   _arrowAnimator;
    private Vector3    _arrowTargetXZ;

    private static readonly int ArrowIdle = Animator.StringToHash("Tutorial_Arrow_Idle");

    private void Awake() => Instance = this;

    private void Start()
    {
        if (_arrowPrefab != null)
        {
            _arrowInstance = Instantiate(_arrowPrefab);
            _arrowAnimator = _arrowInstance.GetComponent<Animator>();
            _arrowInstance.SetActive(false);
        }

        foreach (var step in _steps)
        {
            if (step.zone        != null) step.zone.gameObject.SetActive(false);
            if (step.arrowAnchor != null) step.arrowAnchor.gameObject.SetActive(false);
        }
        if (_compass != null) _compass.gameObject.SetActive(false);

        AdvanceToNext();
    }

    private void LateUpdate()
    {
        if (_arrowInstance == null || !_arrowInstance.activeSelf) return;
        Vector3 pos = _arrowInstance.transform.position;
        pos.x = _arrowTargetXZ.x;
        pos.z = _arrowTargetXZ.z;
        _arrowInstance.transform.position = pos;
    }

    private void Update()
    {
        if (_compass == null || _arrowInstance == null || !_arrowInstance.activeSelf)
        {
            if (_compass != null) _compass.gameObject.SetActive(false);
            return;
        }

        PlayerController player = GameManager.Instance.Player;
        if (player == null) return;

        float dist    = Vector3.Distance(player.transform.position, _arrowInstance.transform.position);
        bool  farAway = dist > _compassShowDistance;

        _compass.gameObject.SetActive(farAway);

        if (farAway)
        {
            Vector3 dir = _arrowInstance.transform.position - player.transform.position;
            dir.y = 0f;
            if (dir != Vector3.zero)
                _compass.rotation = Quaternion.LookRotation(dir);
        }
    }

    // 외부 스크립트(UI, 상점 등)에서 조건 충족 시 호출
    public void CompleteCurrentStep()
    {
        if (_currentIndex < 0 || _currentIndex >= _steps.Count) return;

        TutorialStep step = _steps[_currentIndex];
        if (step.zone != null) step.zone.gameObject.SetActive(false);

        AdvanceToNext();
    }

    private void AdvanceToNext()
    {
        if (_currentIndex >= 0 && _currentIndex < _steps.Count)
        {
            var prev = _steps[_currentIndex];
            if (prev.arrowAnchor != null) prev.arrowAnchor.gameObject.SetActive(false);
        }

        _currentIndex++;

        if (_currentIndex >= _steps.Count)
        {
            if (_arrowInstance != null) _arrowInstance.SetActive(false);
            if (_compass       != null) _compass.gameObject.SetActive(false);
            return;
        }

        TutorialStep step = _steps[_currentIndex];

        if (step.arrowAnchor != null)
        {
            step.arrowAnchor.gameObject.SetActive(true);

            if (_arrowInstance != null)
            {
                _arrowTargetXZ = step.arrowAnchor.position;
                _arrowInstance.SetActive(false);
                _arrowInstance.transform.position = step.arrowAnchor.position;
                _arrowInstance.SetActive(true);
                if (_arrowAnimator != null) _arrowAnimator.Play(ArrowIdle);
            }
        }

        if (step.zone != null)
            step.zone.gameObject.SetActive(true);
    }
}
