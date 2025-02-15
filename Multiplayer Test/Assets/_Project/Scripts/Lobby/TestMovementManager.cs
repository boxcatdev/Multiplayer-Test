using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;

public class TestMovementManager : NetworkBehaviour
{
    public static TestMovementManager Instance;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {

    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsServer) return;

    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerConnectedRpc(ulong clientId)
    {
        Debug.Log($"{clientId} connected");
    }

    [Rpc(SendTo.Server)]
    public void MovePlayerRpc(ulong clientId, Vector3 playerPosition)
    {
        Transform player = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.transform;

        player.position = playerPosition;
    }
}
