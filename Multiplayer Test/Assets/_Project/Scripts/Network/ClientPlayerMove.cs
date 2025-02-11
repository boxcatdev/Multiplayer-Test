using Unity.Netcode;
using UnityEngine;

public class ClientPlayerMove : NetworkBehaviour
{
    private InputHandler inputHandler;
    private TopDownController topDownController;

    private void Awake()
    {
        inputHandler = GetComponent<InputHandler>();
        topDownController = GetComponent<TopDownController>();

        if (inputHandler != null) inputHandler.enabled = false;
        if (topDownController != null) topDownController.enabled = false;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            if (inputHandler != null) inputHandler.enabled = true;
            if (topDownController != null) topDownController.enabled = true;
        }
    }
}
