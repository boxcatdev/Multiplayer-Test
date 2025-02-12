using System;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public enum PlayerType { None, Cross, Circle }

    public static GameManager Instance { get; private set; }

    private PlayerType _localPlayerType;
    public PlayerType localPlayerType => _localPlayerType;

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

        if (NetworkManager.Singleton.LocalClientId == 0) _localPlayerType = PlayerType.Cross;
        else _localPlayerType = PlayerType.Circle;

        Debug.Log("Player type: " + _localPlayerType.ToString());
    }

    public void ClickedOnGridPosition(int x, int y)
    {
        Debug.Log("Clicked on " + x + ", " + y);

        OnClickedOnGridPosition?.Invoke(x, y, _localPlayerType);
    }
}

