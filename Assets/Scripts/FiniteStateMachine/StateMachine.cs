using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

[RequireComponent(typeof(IdleState))]
[RequireComponent(typeof(MoveState))]
[RequireComponent(typeof(ShootState))]
public class StateMachine : MonoBehaviour {
    private State _currentState;
    private IdleState _idleState;
    private MoveState _moveState;
    private ShootState _shootState;
    private Transform _target;

    public Transform Target => _target;

    private void Awake() {
        _idleState = GetComponent<IdleState>();
        _idleState.enabled = false;
        _moveState = GetComponent<MoveState>();
        _moveState.enabled = false;
        _shootState = GetComponent<ShootState>();
        _shootState.enabled = false;

        _currentState = _idleState;

        _target = FindObjectOfType<PlayerController>().gameObject.transform;
    }

    private void Start() {
        _currentState.Enter();
    }

    public void ChangeState(States state) {
        _currentState.Exit();

        switch (state) {
            case States.Idle:
                _currentState = _idleState;
                break;
            case States.Move:
                _currentState = _moveState;
                break;
            case States.Shoot:
                _currentState = _shootState;
                break;
        }

        _currentState.Enter();
    }

    private void FixedUpdate() {
        _currentState.FixedUpdate();
    }

    public void Update() {
        _currentState.Update();
    }

    public bool HasLineOfSightToTarget() {
        bool lineOfSight = false;
        Ray ray = new Ray(transform.position, _target.position - transform.position);
        if (Physics.Raycast(ray, out var raycastHit, 10f)) {
            var playerController = raycastHit.collider.gameObject.GetComponent<PlayerController>();
            lineOfSight = playerController != null;
        }

        return lineOfSight;
    }

    public bool CanShoot() {
        return _shootState.CanShoot();
    }
}
