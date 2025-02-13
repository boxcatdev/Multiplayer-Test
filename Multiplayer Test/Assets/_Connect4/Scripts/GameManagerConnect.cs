using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManagerConnect : NetworkBehaviour
{
    public static GameManagerConnect Instance;

    public enum PlayerType { None, Red, Yellow}

    [Header("Transforms")]
    [SerializeField] private List<GridColumn> _gridColumns = new List<GridColumn>();
    [Space]
    [SerializeField] private GameChip _currentChip;

    private PlayerType _localPlayerType;
    private PlayerType[,] _playerTypeArray;

    public NetworkVariable<PlayerType> currentPlayerTurn = new NetworkVariable<PlayerType>();
    public NetworkVariable<int> playerRedScore = new NetworkVariable<int>();
    public NetworkVariable<int> playerYellowScore = new NetworkVariable<int>();

    public NetworkVariable<bool> waitingMovement = new NetworkVariable<bool>();

    // events
    public Action<GridColumn, PlayerType> OnColumnHover = delegate { };
    public Action<GridColumn, PlayerType> OnColumnSelect = delegate { };

    public Action OnGameStart = delegate { };
    public Action OnGameWin = delegate { };
    public Action OnGameTied = delegate { };
    public Action OnGameRematch = delegate { };

    public Action OnCurrentTurnValueChanged = delegate { };
    public Action OnScoreValueChanged = delegate { };

    private void Awake()
    {
        #region Singleton
        Instance = this;
        #endregion

        _playerTypeArray = new PlayerType[7,6];
    }

    // network
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (NetworkManager.Singleton.LocalClientId == 0) _localPlayerType = PlayerType.Red;
        else _localPlayerType = PlayerType.Yellow;

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManagerConnectedCallback;
        }

        // network variable listeners
        currentPlayerTurn.OnValueChanged += GM_PlayerTurnChangedListener;
        playerRedScore.OnValueChanged += GM_ScoreChangedListener;
        playerYellowScore.OnValueChanged += GM_ScoreChangedListener;
    }

    private void NetworkManagerConnectedCallback(ulong obj)
    {
        if (NetworkManager.Singleton.ConnectedClientsList.Count == 2)
        {
            currentPlayerTurn.Value = PlayerType.Red;
            TriggerOnGameStartRpc();
        }
    }

    private void GM_PlayerTurnChangedListener(PlayerType prevPlayerType, PlayerType newPlayerType)
    {
        OnCurrentTurnValueChanged?.Invoke();
    }
    private void GM_ScoreChangedListener(int prevScore, int newScore)
    {
        OnScoreValueChanged?.Invoke();
    }

    // server rpcs
    [Rpc(SendTo.Server)]
    public void HoverOnColumnRpc(int columnIndex, PlayerType playerType)
    {
        if (columnIndex >= _gridColumns.Count) return;

        GridColumn column = _gridColumns[columnIndex];
        Debug.Log("GM Hover " + column.name);

        // spawn event
        OnColumnHover?.Invoke(column, playerType);
    }
    [Rpc(SendTo.Server)]
    public void SelectColumnRpc(int columnIndex, PlayerType playerType)
    {
        if (columnIndex >= _gridColumns.Count) return;

        GridColumn column = _gridColumns[columnIndex];
        Debug.Log("GM Select " + column.name);

        // click event
        OnColumnSelect?.Invoke(column, playerType);
    }
    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameStartRpc()
    {
        OnGameStart?.Invoke();
    }

    // references
    public PlayerType GetLocalPlayerType()
    {
        return _localPlayerType;
    }
    public List<GridColumn> GetGridColumnList()
    {
        return _gridColumns;
    }
    public GameChip GetCurrentChip()
    {
        return _currentChip;
    }
    public void SetCurrentChip(GameChip gameChip)
    {
        _currentChip = gameChip;
    }

    public PlayerType[,] GetPlayerTypeArray()
    {
        return _playerTypeArray;
    }
    public void SetPlayerTypeInArray(PlayerType playerType, int x, int y)
    {
        _playerTypeArray[x, y] = playerType;
    }
}
