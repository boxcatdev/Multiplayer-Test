using Unity.Netcode;
using UnityEngine;

public class ClientPlayerMove : NetworkBehaviour
{
    private InputHandler inputHandler;
    private TopDownController topDownController;

    [SerializeField] bool _isServerAuth = false;

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

            if (_isServerAuth == false)
            {
                if (topDownController != null) topDownController.enabled = true;
            }
        }

        if (_isServerAuth == false) return;

        // server auth
        if (IsServer)
        {
            if (topDownController != null) topDownController.enabled = true;
        }
    }

    [Rpc(SendTo.Server)]
    private void UpdateInputServerRPC(Vector2 move)
    {
        inputHandler.MoveInput(move);
    }

    private void Update()
    {
        if (!IsOwner) return;
        if (_isServerAuth == false) return;

        UpdateInputServerRPC(inputHandler.move);
    }
}
