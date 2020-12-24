using UnityEngine;

public class Weapon : MonoBehaviour {
    [SerializeField] private Transform spawnPoint;
    private Timer _shotTimer = new Timer();
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private float fireRate;

    private void Awake() {
        _shotTimer.RunningTime = fireRate;
    }

    public void Shoot() {
        if (_shotTimer.Finished()) {
            _shotTimer.Start();

            Instantiate(_projectilePrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}
