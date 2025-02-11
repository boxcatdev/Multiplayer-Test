using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MoveTransform : MovementBaseTransform
{
    protected override Vector3 GetMovePosition()
    {
        Vector3 dir = transform.position - _targetPosition;
        return transform.position - dir.normalized * (_speedOverwrite ? _overwriteSpeed : _pairs[_currentPos].speed);
    }
}
