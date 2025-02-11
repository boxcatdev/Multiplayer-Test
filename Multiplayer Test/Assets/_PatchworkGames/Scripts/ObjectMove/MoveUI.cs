using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MoveUI : MovementBaseUI
{
    protected override Vector2 GetMovePosition()
    {
        Vector2 dir = _rect.anchoredPosition - _targetPosition;
        return _rect.anchoredPosition - dir.normalized * (_speedOverwrite ? _overwriteSpeed : _pairs[_currentPos].speed);
    }
}
