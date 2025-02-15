using PatchworkGames;
using System;
using Unity.Netcode;
using UnityEngine;

public class TestSphere : NetworkBehaviour
{
    public static Action OnChangeColor = delegate { };

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        OnChangeColor += ChangeColor;

        OnChangeColor?.Invoke();
    }

    private void Start()
    {
        Debug.Log(name + " " + OwnerClientId.ToString());
    }
    private void Update()
    {
        if (!IsOwner) return;

        HandleMovement();
    }

    private void ChangeColor()
    {
        transform.GetChild((int)OwnerClientId).gameObject.SetActive(false);
    }
    private void HandleMovement()
    {
        Vector3 pos = Utility.GetMouseHitPosition3D(Camera.main);

        if (pos != Vector3.zero)
        {
            ulong id = OwnerClientId;
            TestMovementManager.Instance.MovePlayerRpc(id, pos);
        }
    }
}
