using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetupState : State
{
    GameFSM _statemachine;
    GameController _controller;

    public GameSetupState(GameFSM stateMachine, GameController controller)
    {
        _statemachine = stateMachine;
        _controller = controller;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Update()
    {
        base.Update();
    }
}
