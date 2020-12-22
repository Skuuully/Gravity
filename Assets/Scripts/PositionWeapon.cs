using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionWeapon : MonoBehaviour {
    [SerializeField] private Transform _weaponTransform;

    void FixedUpdate() {
        Plane plane = new Plane(transform.up, transform.position);
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(mouseRay, out float enterPoint)) {
            Vector3 mousePosition = mouseRay.GetPoint(enterPoint);
            Vector3 direction = mousePosition - transform.position;
            direction.Normalize();
            direction *= 1.3f;
            _weaponTransform.position = transform.position + direction;
            _weaponTransform.LookAt(mousePosition, transform.up);
        }

    }
}
