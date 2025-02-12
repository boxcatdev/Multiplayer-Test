using System;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public enum PlayerType { None, Cross, Circle }

    public static GameManager Instance { get; private set; }

    private PlayerType localPlayerType;
    private PlayerType currentTurnPlayerType;

    public PlayerType LocalPlayerType => localPlayerType;

    public Action<int, int, PlayerType> OnClickedOnGridPosition = delegate { };

    private void Awake()
    {
        #region Singleton
        Instance = this;

        /*if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            if (Instance != this) Destroy(gameObject);
        }*/
        #endregion
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (NetworkManager.Singleton.LocalClientId == 0) localPlayerType = PlayerType.Cross;
        else localPlayerType = PlayerType.Circle;

        Debug.Log("Player type: " + localPlayerType.ToString());

        // only the server's version of this property matters
        if (IsServer)
        {
            currentTurnPlayerType = PlayerType.Cross;
        }
    }

    [Rpc(SendTo.Server)]
    public void ClickedOnGridPositionRpc(int x, int y, PlayerType playerType)
    {
        // check if current player turn
        if (playerType != currentTurnPlayerType) return;

        Debug.Log("Clicked on " + x + ", " + y);

        // visual event
        OnClickedOnGridPosition?.Invoke(x, y, playerType);

        // switch turns
        if (currentTurnPlayerType == PlayerType.Cross) currentTurnPlayerType = PlayerType.Circle;
        else currentTurnPlayerType = PlayerType.Cross;
    }
}

