using System;
using UnityEngine;

public interface IInteractable
{
    void OnInteract(Transform transform);
    void OnHoverEnter();
    void OnHoverExit();
    void SetCanInteract(bool canInteract);
    bool GetCanInteract();
    string GetInteractText();
    Transform GetTransform();
}
