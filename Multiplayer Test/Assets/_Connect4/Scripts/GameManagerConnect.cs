using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManagerConnect : NetworkBehaviour
{
    public static GameManagerConnect Instance;

    public enum PlayerType { None, Red, Yellow }

    [Header("Transforms")]
    [SerializeField] private List<GridColumn> _gridColumns = new List<GridColumn>();
    [Space]
    [SerializeField] private GameChip _currentChip;

    private PlayerType _localPlayerType;
    private PlayerType[,] _playerTypeArray;

    // lines
    private List<Vector2Int[]> _winningLineOffsets = new List<Vector2Int[]>()
    {
        // current xy not included
        new Vector2Int[3] {new Vector2Int(0,1), new Vector2Int(0,2), new Vector2Int(0,3)}, // north
        new Vector2Int[3] {new Vector2Int(1,0), new Vector2Int(2,0), new Vector2Int(3,0)}, // east
        new Vector2Int[3] {new Vector2Int(0,-1), new Vector2Int(0,-2), new Vector2Int(0,-3)}, // south
        new Vector2Int[3] {new Vector2Int(-1,0), new Vector2Int(-2,0), new Vector2Int(-3,0)}, // west
        
        new Vector2Int[3] {new Vector2Int(1,1), new Vector2Int(2,2), new Vector2Int(3,3)}, // northeast
        new Vector2Int[3] {new Vector2Int(1,-1), new Vector2Int(2,-2), new Vector2Int(3,-3)}, // southeast
        new Vector2Int[3] {new Vector2Int(-1,-1), new Vector2Int(-2,-2), new Vector2Int(-3,-3)}, // southwest
        new Vector2Int[3] {new Vector2Int(-1,1), new Vector2Int(-2,2), new Vector2Int(-3,3)}, // northwest
    };

    // network variables
    public NetworkVariable<PlayerType> currentPlayerTurn = new NetworkVariable<PlayerType>();
    public NetworkVariable<int> playerRedScore = new NetworkVariable<int>();
    public NetworkVariable<int> playerYellowScore = new NetworkVariable<int>();

    public NetworkVariable<bool> waitingMovement = new NetworkVariable<bool>();

    // events
    public Action<GridColumn, PlayerType> OnColumnHover = delegate { };
    public Action<int, int, PlayerType> OnColumnSelect = delegate { };

    public Action OnGameStart = delegate { };
    public Action<PlayerType> OnGameWin = delegate { };
    public Action OnGameDraw = delegate { };
    public Action OnGameRematch = delegate { };

    public Action OnCurrentTurnValueChanged = delegate { };
    public Action OnScoreValueChanged = delegate { };

    public Action OnPlayerDisconnected = delegate { };

    private void Awake()
    {
        #region Singleton
        Instance = this;
        #endregion

        _playerTypeArray = new PlayerType[VisualManagerConnect.GRID_WIDTH, VisualManagerConnect.GRID_HEIGHT];
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
    public void HoverOnColumnRpc(int columnIndex, PlayerType localPlayerType)
    {
        if (columnIndex >= _gridColumns.Count) return;
        if (localPlayerType != currentPlayerTurn.Value) return;


        GridColumn column = _gridColumns[columnIndex];
        Debug.Log("GM Hover " + column.name);

        // spawn event
        OnColumnHover?.Invoke(column, localPlayerType);
    }
    [Rpc(SendTo.Server)]
    public void SelectColumnRpc(int columnIndex, PlayerType localPlayerType)
    {
        if (columnIndex >= _gridColumns.Count) return;
        if (localPlayerType != currentPlayerTurn.Value) return;

        if (waitingMovement.Value == true) return;
        if (_currentChip == null) return;

        GridColumn column = _gridColumns[columnIndex];
        Debug.Log("GM Select " + column.name);


        // find free slot in column
        for (int y = 0; y < VisualManagerConnect.GRID_HEIGHT; y++)
        {
            if (_playerTypeArray[columnIndex, y] == PlayerType.None)
            {
                Debug.Log("Free slot: " + columnIndex + "," + y);

                //waiting
                waitingMovement.Value = true;

                // set type
                _playerTypeArray[columnIndex, y] = localPlayerType;

                // click event
                OnColumnSelect?.Invoke(columnIndex, y, localPlayerType);

                // switch turn
                if (currentPlayerTurn.Value == PlayerType.Red) currentPlayerTurn.Value = PlayerType.Yellow;
                else currentPlayerTurn.Value = PlayerType.Red;



                break;
            }
        }

        
    }
    [Rpc(SendTo.Server)]
    public void CheckWinnerRpc(PlayerType winningPlayerType)
    {
        Debug.Log("CheckWinner()");

        // for each chip on the board (of the same type as last placed)
        // check 8 directions
        // if 4 in a row are the same then win

        if (CheckWinnerLoops(winningPlayerType))
        {
            Debug.LogWarning("Game win! " + winningPlayerType);

            // stop switching turns
            currentPlayerTurn.Value = PlayerType.None;

            // increase score
            switch(winningPlayerType)
            {
                case PlayerType.Red:
                    playerRedScore.Value++; break;
                case PlayerType.Yellow:
                    playerYellowScore.Value++; break;
            }

            TriggerOnGameWinRpc(winningPlayerType);
            return;
        }

        // check for a draw
        int totalCount = 0;
        int chipCount = 0;
        for (int x = 0; x < VisualManagerConnect.GRID_WIDTH; x++)
        {
            for (int y = 0; y < VisualManagerConnect.GRID_HEIGHT; y++)
            {
                totalCount++;

                if (_playerTypeArray[x, y] != PlayerType.None)
                {
                    chipCount++;
                }
            }
        }

        if (chipCount == totalCount)
        {
            // draw
            Debug.LogWarning("Draw");

            // stop switching turns
            currentPlayerTurn.Value = PlayerType.None;

            TriggerOnGameDrawRpc();
        }
    }
    private bool CheckWinnerLoops(PlayerType winningPlayerType)
    {
        //bool hasWon = false;
        int width = VisualManagerConnect.GRID_WIDTH;
        int height = VisualManagerConnect.GRID_HEIGHT;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (_playerTypeArray[x, y] != winningPlayerType) continue;
                //if (hasWon == true) break;

                PlayerType xyType = _playerTypeArray[x, y];

                // do foreach loop
                foreach (var line in _winningLineOffsets)
                {
                    int count = 0;
                    foreach (var index in line)
                    {
                        Vector2Int offsetCoords = new Vector2Int(x + index.x, y + index.y);
                        if (offsetCoords.x >= 0 && offsetCoords.x < width && offsetCoords.y >= 0 && offsetCoords.y < height)
                        {
                            if (_playerTypeArray[offsetCoords.x, offsetCoords.y] == winningPlayerType) count++;
                        }
                    }

                    if (count == 3)
                    {
                        //hasWon = true;
                        //break;
                        return true;
                    }
                }
            }
        }

        return false;
    }

    [Rpc(SendTo.Server)]
    public void RematchGameRpc()
    {
        // reset player type array
        for (int x = 0; x < _playerTypeArray.GetLength(0); x++)
        {
            for (int y = 0; y < _playerTypeArray.GetLength(1); y++)
            {
                _playerTypeArray[x,y] = PlayerType.None;
            }
        }

        // reset player turn
        currentPlayerTurn.Value = PlayerType.Red;

        // visuals event
        TriggerOnGameRematchRpc();
    }
    [Rpc(SendTo.ClientsAndHost)]
    public void TriggerPlayerDisconnectedRpc()
    {
        OnPlayerDisconnected?.Invoke();
    }

    // client rpcs
    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameStartRpc()
    {
        OnGameStart?.Invoke();
    }
    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameWinRpc(PlayerType winningPlayerType)
    {
        OnGameWin?.Invoke(winningPlayerType);
    }
    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameDrawRpc()
    {
        OnGameDraw?.Invoke();
    }
    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameRematchRpc()
    {
        OnGameRematch?.Invoke();
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
