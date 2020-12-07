using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Test;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInteractor : MonoBehaviour {
    private List<IInteractable> _interactables;
    private IInteractable _previousClosest;
    private IInteractable _closestInteractable;
    private PlayerInput _playerInput;

    private void Awake() {
        _playerInput = GetComponent<PlayerInput>();
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode) {
        UpdateInteractables();
    }
    private void OnSceneUnloaded(Scene scene) {
        _interactables = new List<IInteractable>();
    }

    private void Start() {
        UpdateInteractables();
    }

    private void FixedUpdate() {
        _closestInteractable = FindClosestInteractable();
        if (_closestInteractable != null) {
            float distance = (transform.position - _closestInteractable.GetPosition()).magnitude;
            if (distance < _closestInteractable.GetInteractRadius()) {
                if (_closestInteractable != null && _playerInput.interact) {
                    _closestInteractable.Interact();
                }

                _closestInteractable.ShowText(true);
                if (_previousClosest != null && _previousClosest != _closestInteractable) {
                    _previousClosest.ShowText(false);
                }
            }
        }
    }

    IInteractable FindClosestInteractable() {
        _previousClosest = _closestInteractable;
        IInteractable nearest = null;
        float minDistance = float.MaxValue;
        foreach (var interactable in _interactables) {
             float currDistance = (interactable.GetPosition() - transform.position).magnitude;
             if (currDistance < minDistance) {
                 nearest = interactable;
                 minDistance = currDistance;
             }
        }

        return nearest;
    }

    void UpdateInteractables() {
        _interactables = FindObjectsOfType<MonoBehaviour>().OfType<IInteractable>().ToList();
    }
}
