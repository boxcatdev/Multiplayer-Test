using System;
using System.Collections.Generic;
using PatchworkGames;
using TMPro;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    private InputHandler _input;

    [Header("Interaction")]
    [SerializeField] private float _interactRadius = 0.35f;
    [Space]
    [SerializeField] private Transform _interactSource;

    private IInteractable _currentInteractable;
    public IInteractable currentInteractable => _currentInteractable;

    [Header("UI")]
    [SerializeField] private InteractUI _uiController;

    public Action<bool> OnUiUpdated = delegate { };
    public static Action<bool> OnShowInteractPrompt = delegate { };

    private void Awake()
    {
        _input = GetComponent<InputHandler>();
    }
    private void OnEnable()
    {
        _input.OnUsePress += Interact;
        OnShowInteractPrompt += ShowInteractPrompt;
    }
    private void OnDisable()
    {
        _input.OnUsePress -= Interact;
        OnShowInteractPrompt -= ShowInteractPrompt;
    }

    private void Update()
    {
        Hover();
    }

    private void Hover()
    {
        Vector3 hitPosition = _interactSource.position;

        Collider[] colliders = Physics.OverlapSphere(hitPosition, _interactRadius);
        if (colliders.Length > 0)
        {
            List<IInteractable> interactables = new List<IInteractable>();
            IInteractable closestInteractable = null;
            foreach (Collider collider in colliders)
            {
                IInteractable interactable = collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactables.Add(interactable);

                    if (closestInteractable == null)
                    {
                        closestInteractable = interactable;
                    }
                    else
                    {
                        if (Vector3.Distance(hitPosition, interactable.GetTransform().position) < Vector3.Distance(hitPosition, closestInteractable.GetTransform().position))
                        {
                            closestInteractable = interactable;
                        }
                    }
                }
            }

            if (_currentInteractable != closestInteractable)
            {
                if (_currentInteractable != null) _currentInteractable.OnHoverExit();
                if (closestInteractable != null) closestInteractable.OnHoverEnter();

                _currentInteractable = closestInteractable;

                UpdateInteractText();
            }
        }
        else
        {
            if (_currentInteractable != null)
            {
                _currentInteractable = null;

                UpdateInteractText();
            }
        }
    }
    private void Interact()
    {
        if (_currentInteractable == null) return;

        Debug.Log("Interact");
        _currentInteractable.OnInteract(transform);
    }
    private void ShowInteractPrompt(bool show)
    {
        if (_uiController == null) return;

        _uiController.EnablePrompt(show);
    }
    private void UpdateInteractText()
    {
        if (_uiController == null) return;

        if (_currentInteractable != null)
        {
            if (_currentInteractable.GetCanInteract())
            {
                _uiController.EnablePrompt(true);
                _uiController.SetPromptText(_currentInteractable.GetInteractText());

                OnUiUpdated?.Invoke(true);
            }
            else
            {
                _uiController.EnablePrompt(false);
                _uiController.SetPromptText("");

                OnUiUpdated?.Invoke(false);
            }
        }
        else
        {
            _uiController.EnablePrompt(false);
            _uiController.SetPromptText("");

            OnUiUpdated?.Invoke(false);
        }

        /*if (_interactText == null) return;

        if (_currentInteractable != null)
        {
            //_interactUI.SetActive(true);
            _uiController.EnableAll(true);

            OnUiUpdated?.Invoke(true);
            //_interactText.gameObject.SetActive(true);
            //_backupText.gameObject.SetActive(true);

            _interactText.text = _currentInteractable.GetInteractText();
            _backupText.text = _currentInteractable.GetInteractText();
        }
        else
        {
            _uiController.EnableAll(false);
            //_interactUI.SetActive(false);
            OnUiUpdated?.Invoke(false);
            //_interactText.gameObject.SetActive(false);
            //_backupText.gameObject.SetActive(false);
        }*/
    }

    private void OnDrawGizmos()
    {
        if (_interactSource != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_interactSource.position, _interactRadius);
        }
    }
}

