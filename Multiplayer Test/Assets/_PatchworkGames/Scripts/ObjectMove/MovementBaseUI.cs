using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class MovementBaseUI : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] protected bool _speedOverwrite = false;
    [SerializeField] protected float _overwriteSpeed = 1f;
    [Space]
    [SerializeField] protected float _buffer = 0.1f;
    [Space]
    [SerializeField] protected int _currentPos = 0;
    [SerializeField] protected SpeedPairVector2[] _pairs;

    protected RectTransform _rect;
    protected Vector2 _targetPosition;

    [Header("Events")]
    public UnityEvent OnAtTargetPositionUE;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }
    private void FixedUpdate()
    {
        UpdateTarget();

        if (_rect.anchoredPosition != _targetPosition)
        {
            if (Vector2.Distance(_rect.anchoredPosition, _targetPosition) > (_overwriteSpeed > _buffer ? _overwriteSpeed : _buffer))
            {
                _rect.anchoredPosition = GetMovePosition();
            }
            else
            {
                _rect.anchoredPosition = _targetPosition;
                OnAtTargetPositionUE?.Invoke();
            }
        }
    }
    protected abstract Vector2 GetMovePosition();
    protected void UpdateTarget()
    {
        if (_pairs.Length > 0)
        {
            if (_rect.anchoredPosition != _pairs[_currentPos].position)
            {
                _targetPosition = _pairs[_currentPos].position;
            }
        }
    }
    public void NextTarget()
    {
        if (_pairs.Length > 0)
        {
            _currentPos = (_currentPos + 1) % _pairs.Length;
        }
    }
}

[Serializable]
public struct SpeedPairVector2
{
    public float speed;
    public Vector2 position;
}