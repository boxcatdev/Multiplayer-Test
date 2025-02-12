using System;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public enum PlayerType { None, Cross, Circle }

    public static GameManager Instance { get; private set; }

    public PlayerType localPlayerType { get; private set; }
    public NetworkVariable<PlayerType> currentTurnPlayerType = new NetworkVariable<PlayerType>();


    public Action<int, int, PlayerType> OnClickedOnGridPosition = delegate { };
    
    public Action OnGameStarted = delegate { };
    public Action OnCurrentPlayerTurnChanged = delegate { };

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

        if (IsServer)
        {
            // event when any player joins
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManagerConnectedCallback;
        }

        // network variable listener
        currentTurnPlayerType.OnValueChanged += CurrentPlayerTurnValueChanged;
    }

    private void CurrentPlayerTurnValueChanged(PlayerType previousValue, PlayerType newValue)
    {
        OnCurrentPlayerTurnChanged?.Invoke();
    }

    private void NetworkManagerConnectedCallback(ulong obj)
    {
        // check if can start game
        if(NetworkManager.Singleton.ConnectedClientsList.Count == 2)
        {
            currentTurnPlayerType.Value = PlayerType.Cross;
            TriggerOnGameStartedRpc();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameStartedRpc()
    {
        OnGameStarted?.Invoke();
    }

    [Rpc(SendTo.Server)]
    public void ClickedOnGridPositionRpc(int x, int y, PlayerType playerType)
    {
        // check if current player turn
        if (playerType != currentTurnPlayerType.Value) return;

        Debug.Log("Clicked on " + x + ", " + y);

        // visual event
        OnClickedOnGridPosition?.Invoke(x, y, playerType);

        // switch turns
        if (currentTurnPlayerType.Value == PlayerType.Cross) currentTurnPlayerType.Value = PlayerType.Circle;
        else currentTurnPlayerType.Value = PlayerType.Cross;

        // already listening for value changed
    }
}

