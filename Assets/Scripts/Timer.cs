using UnityEngine;

public class Timer {
    private float _startTime = 0f;
    private float _runningTime = 1f;

    public float RunningTime {
        set => _runningTime = value;
    }

    public Timer(float runningTime = 1) {
        _runningTime = runningTime;
    }

    public void Start() {
        _startTime = Time.time;
    }

    public bool Finished() {
        return Time.time >= (_runningTime + _startTime);
    }
}
