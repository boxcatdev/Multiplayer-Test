using System;
using Unity.Netcode;
using UnityEngine;

public class GameChip : NetworkBehaviour
{
    private GameManagerConnect.PlayerType _playerType;
    public GameManagerConnect.PlayerType playerType => _playerType;

    [SerializeField] private Transform redChip;
    [SerializeField] private Transform yellowChip;

    public Action<GameManagerConnect.PlayerType> OnSetPlayerType = delegate { };

    private void OnEnable()
    {
        OnSetPlayerType += SetPlayerType;
    }
    private void OnDisable()
    {
        OnSetPlayerType -= SetPlayerType;
    }
    private void SetPlayerType(GameManagerConnect.PlayerType playerType)
    {
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
