using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class MovementBaseTransform : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] protected bool _speedOverwrite = false;
    [SerializeField] protected float _overwriteSpeed = 1f;
    [Space]
    [SerializeField] protected float _buffer = 0.1f;
    [Space]
    [SerializeField] protected int _currentPos = 0;
    [SerializeField] protected SpeedPairVector3[] _pairs;

    protected Vector3 _targetPosition;

    [Header("Events")]
    public UnityEvent OnAtTargetPositionUE;

    private void FixedUpdate()
    {
        UpdateTarget();

        if (transform.position != _targetPosition)
        {
            if (Vector3.Distance(transform.position, _targetPosition) > (_overwriteSpeed > _buffer ? _overwriteSpeed : _buffer))
            {
                transform.position = GetMovePosition();
            }
            else
            {
                transform.position = _targetPosition;
                OnAtTargetPositionUE?.Invoke();
            }
        }
    }
    protected abstract Vector3 GetMovePosition();
    protected void UpdateTarget()
    {
        if (_pairs.Length > 0)
        {
            if (_targetPosition != _pairs[_currentPos].position)
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
public struct SpeedPairVector3
{
    public float speed;
    public Vector3 position;
}
