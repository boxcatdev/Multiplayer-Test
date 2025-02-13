using System;
using UnityEngine;

public class GameChip : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [Space]
    [SerializeField] private Transform redChip;
    [SerializeField] private Transform yellowChip;

    public void SetPlayerType(GameManagerConnect.PlayerType playerType)
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
