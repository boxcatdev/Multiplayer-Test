using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public enum PlayerType { None, Cross, Circle }
    public enum Orientation { Horizontal, Vertical, DiagonalA, DiagonalB}
    public struct Line
    {
        public Vector2Int centerGridPosition;
        public List<Vector2Int> lineGridPositions;
        public Orientation orientation;
    }

    public static GameManager Instance { get; private set; }

    public PlayerType localPlayerType { get; private set; }
    public NetworkVariable<PlayerType> currentTurnPlayerType = new NetworkVariable<PlayerType>();

    private PlayerType[,] playerTypeArray;
    private List<Line> lineList;


    public Action<int, int, PlayerType> OnClickedOnGridPosition = delegate { };
    
    public Action OnGameStarted = delegate { };
    public Action<Line> OnGameWin = delegate { };
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

        playerTypeArray = new PlayerType[3,3];
        lineList = new List<Line>()
        {
            // vertical
            new Line()
            {
                centerGridPosition = new Vector2Int(0, 1),
                lineGridPositions = new List<Vector2Int>() {new Vector2Int(0,0), new Vector2Int(0,1), new Vector2Int(0,2)},
                orientation = Orientation.Vertical
            },
            new Line()
            {
                centerGridPosition = new Vector2Int(1, 1),
                lineGridPositions = new List<Vector2Int>() {new Vector2Int(1,0), new Vector2Int(1,1), new Vector2Int(1,2)},
                orientation = Orientation.Vertical
            },
            new Line()
            {
                centerGridPosition = new Vector2Int(2, 1),
                lineGridPositions = new List<Vector2Int>() {new Vector2Int(2,0), new Vector2Int(2,1), new Vector2Int(2,2)},
                orientation = Orientation.Vertical
            },
            // horizontal
            new Line()
            {
                centerGridPosition = new Vector2Int(1,0),
                lineGridPositions = new List<Vector2Int>() {new Vector2Int(0,0), new Vector2Int(1,0), new Vector2Int(2,0)},
                orientation = Orientation.Horizontal
            },
            new Line()
            {
                centerGridPosition = new Vector2Int(1, 1),
                lineGridPositions = new List<Vector2Int>() {new Vector2Int(1,0), new Vector2Int(1,1), new Vector2Int(2,1)},
                orientation = Orientation.Horizontal
            },
            new Line()
            {
                centerGridPosition = new Vector2Int(1, 2),
                lineGridPositions = new List<Vector2Int>() {new Vector2Int(0,2), new Vector2Int(1,2), new Vector2Int(2,2)},
                orientation = Orientation.Horizontal
            },
            // diagonal
            new Line()
            {
                centerGridPosition = new Vector2Int(1,1),
                lineGridPositions = new List<Vector2Int>() {new Vector2Int(0,0), new Vector2Int(1,1), new Vector2Int(2,2)},
                orientation = Orientation.DiagonalB
            },
            new Line()
            {
                centerGridPosition = new Vector2Int(1,1),
                lineGridPositions = new List<Vector2Int>() {new Vector2Int(0,2), new Vector2Int(1,1), new Vector2Int(2,0)},
                orientation = Orientation.DiagonalA
            }
        };
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

    private bool CheckWinnerLine(Line line)
    {
        return CheckWinnerLine(playerTypeArray[line.lineGridPositions[0].x, line.lineGridPositions[0].y],
                                playerTypeArray[line.lineGridPositions[1].x, line.lineGridPositions[1].y],
                                playerTypeArray[line.lineGridPositions[2].x, line.lineGridPositions[2].y]);
    }
    private bool CheckWinnerLine(PlayerType aPlayerType, PlayerType bPlayerType, PlayerType cPlayerType)
    {
        return aPlayerType != PlayerType.None && aPlayerType == bPlayerType && bPlayerType == cPlayerType;
    }
    // server only
    private void CheckWinner()
    {
        foreach (var line in lineList)
        {
            if (CheckWinnerLine(line))
            {
                Debug.Log($"Winner {playerTypeArray[0, 0]}");
                currentTurnPlayerType.Value = PlayerType.None;

                OnGameWin?.Invoke(line);
                break;
            }
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

        // check if a piece has already been placed
        if (playerTypeArray[x, y] != PlayerType.None) return;

        Debug.Log("Clicked on " + x + ", " + y);

        // set playertype on position
        playerTypeArray[x, y] = playerType;

        // visual event
        OnClickedOnGridPosition?.Invoke(x, y, playerType);

        // switch turns
        if (currentTurnPlayerType.Value == PlayerType.Cross) currentTurnPlayerType.Value = PlayerType.Circle;
        else currentTurnPlayerType.Value = PlayerType.Cross;

        // already listening for value changed

        // test winner
        CheckWinner();
    }
}

