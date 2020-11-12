using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour {
    private float _startTime = 0f;
    [SerializeField] private float _runningTime = 1f;

    public void Initialise(float runningTime) {
        _runningTime = runningTime;
    }

    public void Start() {
        _startTime = Time.time;
    }

    public bool Finished() {
        return Time.time >= (_runningTime + _startTime);
    }
}
