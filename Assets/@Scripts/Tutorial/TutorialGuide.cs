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

    private int        _currentIndex  = -1;
    private GameObject _arrowInstance;
    private Animator   _arrowAnimator;
    private Vector3    _arrowTargetXZ;

    private static readonly int ArrowIdle = Animator.StringToHash("Tutorial_Arrow_Idle");

    // CompassArrow가 사용하는 안정적 목표 위치
    public bool    HasActiveArrow      => _arrowInstance != null && _arrowInstance.activeSelf;
    public Vector3 ArrowTargetPosition => _arrowTargetXZ;

    private void Awake() => Instance = this;

    private void Start()
    {
        if (_arrowPrefab != null)
        {
            _arrowInstance      = Instantiate(_arrowPrefab);
            _arrowInstance.name = "Tutorial(Arrow)";
            _arrowAnimator      = _arrowInstance.GetComponent<Animator>();
            _arrowInstance.SetActive(false);
        }

        foreach (var step in _steps)
        {
            if (step.zone        != null) step.zone.gameObject.SetActive(false);
            if (step.arrowAnchor != null) step.arrowAnchor.gameObject.SetActive(false);
        }

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
