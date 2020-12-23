using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State {
    public override void Enter() {
        _stateMachine.ChangeState(_stateMachine.HasLineOfSightToTarget() ? States.Shoot : States.Move);
    }

    public override void Update() { }
    public override void FixedUpdate() { }
    public override void Exit() { }
}
