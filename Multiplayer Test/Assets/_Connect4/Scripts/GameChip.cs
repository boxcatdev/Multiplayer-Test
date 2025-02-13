using System;
using Unity.Netcode;
using UnityEngine;

public class GameChip : NetworkBehaviour
{
    [SerializeField]
    private GameManagerConnect.PlayerType _playerType;
    public GameManagerConnect.PlayerType playerType => _playerType;

    [Space]
    [SerializeField] private Transform redChip;
    [SerializeField] private Transform yellowChip;

    //public Action<GameManagerConnect.PlayerType> OnSetPlayerType = delegate { };

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        SetPlayerType(GameManagerConnect.Instance.currentPlayerTurn.Value);
        
    }

    private void SetPlayerType(GameManagerConnect.PlayerType playerType)
    {
        _playerType = playerType;

        if (playerType == GameManagerConnect.PlayerType.Red)
        {
            if (redChip != null) redChip.gameObject.SetActive(true);
            if (yellowChip != null) yellowChip.gameObject.SetActive(false);
        }
        else
        {
            if (redChip != null) redChip.gameObject.SetActive(false);
            if (yellowChip != null) yellowChip.gameObject.SetActive(true);
        }
    }
}
