using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LerpUI : MovementBaseUI
{
    protected override Vector2 GetMovePosition()
    {
        return Vector2.Lerp(_rect.anchoredPosition, _targetPosition, _speedOverwrite ? _overwriteSpeed : _pairs[_currentPos].speed);
    }
}


