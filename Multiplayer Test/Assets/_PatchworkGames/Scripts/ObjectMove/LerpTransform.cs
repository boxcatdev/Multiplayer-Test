using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LerpTransform : MovementBaseTransform
{
    protected override Vector3 GetMovePosition()
    {
        return Vector3.Lerp(transform.position, _targetPosition, _speedOverwrite ? _overwriteSpeed : _pairs[_currentPos].speed);
    }
}


