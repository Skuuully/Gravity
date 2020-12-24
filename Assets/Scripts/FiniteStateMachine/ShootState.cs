using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootState : State {
    [SerializeField] private GameObject projectile;
    [SerializeField] private float fireRate;
    private Timer _timer = new Timer();

    protected override void Awake() {
        base.Awake();
        _timer.RunningTime = fireRate;
        _timer.Start();
    }

    public bool CanShoot() {
        return _timer.Finished();
    }

    public override void Enter() {
        if (_timer.Finished()) {
            StartCoroutine(nameof(Shoot));
        } else {
            _stateMachine.ChangeState(States.Move);
        }
    }

    public override void Update() {
        
    }

    public override void FixedUpdate() {
        
    }

    public override void Exit() {
        
    }

    private IEnumerator Shoot() {
        _timer.Start();
        Vector3 directionToTarget = GetDirectionToTarget();
        Vector3 spawnPosition = transform.position;
        Quaternion spawnRotation = GetRotationToFaceTarget(directionToTarget);
        Instantiate(projectile, spawnPosition, spawnRotation);
        yield return new WaitForSeconds(0.5f);
        _stateMachine.ChangeState(States.Move);
    }

    Quaternion GetRotationToFaceTarget(Vector3 directionToTarget) {
        return Quaternion.LookRotation(directionToTarget, transform.up);
    }
    
    Vector3 GetDirectionToTarget() {
        Vector3 direction = _stateMachine.Target.position - transform.position;
        direction.Normalize();
        direction -= transform.up;
        return direction;
    }
}
