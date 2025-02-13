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


    // events
    public Action<GridColumn, PlayerType> OnColumnHover = delegate { };
    private void Awake()
    {
        #region Singleton
        Instance = this;
        #endregion

        _playerTypeArray = new PlayerType[7,6];
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
}
