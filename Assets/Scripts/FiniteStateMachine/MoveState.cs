using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FlowFieldFollower))]
[RequireComponent(typeof(Rigidbody))]
public class MoveState : State {
    private Rigidbody _rigidbody;
    private FlowFieldFollower _flowFieldFollower;

    protected override void Awake() {
        base.Awake();
        _rigidbody = GetComponent<Rigidbody>();
        _flowFieldFollower = GetComponent<FlowFieldFollower>();
    }
    
    public override void Enter() {
        _flowFieldFollower.Enabled = true;
    }

    public override void Update() {

    }

    public override void FixedUpdate() {
        if (_stateMachine.HasLineOfSightToTarget() && _stateMachine.CanShoot()) {
            _stateMachine.ChangeState(States.Shoot);
        }
    }

    public override void Exit() {
        _flowFieldFollower.Enabled = false;
    }
}
