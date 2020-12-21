using UnityEngine;

namespace Test {
    public interface IInteractable {
        void Interact();

        Vector3 GetPosition();

        float GetInteractRadius();

        string GetText();
    }
}