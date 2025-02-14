using DG.Tweening;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class VisualManagerConnect : NetworkBehaviour
{
    public const int GRID_WIDTH = 7;
    public const int GRID_HEIGHT = 6;

    private float yOffset = 6.5f;

    [Header("Prefabs")]
    [SerializeField] private GameChip chipPrefab;

    private List<GameObject> visualGameObjects = new List<GameObject>();


    private void Awake()
    {
        visualGameObjects = new List<GameObject>();
    }
    private void Start()
    {
        GameManagerConnect.Instance.OnColumnHover += GM_ColumnHover;
        GameManagerConnect.Instance.OnColumnSelect += GM_ColumnSelect;
        GameManagerConnect.Instance.OnGameRematch += GM_GameRematch;
    }
    private void OnDisable()
    {
        GameManagerConnect.Instance.OnColumnHover -= GM_ColumnHover;
        GameManagerConnect.Instance.OnColumnSelect -= GM_ColumnSelect;
        GameManagerConnect.Instance.OnGameRematch -= GM_GameRematch;
    }


    private void GM_GameRematch()
    {
        foreach (var gameObject in visualGameObjects)
        {
            Destroy(gameObject);
        }
        visualGameObjects.Clear();
    }
    private void GM_ColumnHover(GridColumn column, GameManagerConnect.PlayerType playerType)
    {
        if (GameManagerConnect.Instance.waitingMovement.Value == true) return;

        Vector3 position = column.transform.position + (Vector3.up * yOffset);

        if (GameManagerConnect.Instance.GetCurrentChip() == null)
        {
            // spawn chip
            SpawnChipRpc(position, playerType);
        }
        else
        {
            // move to column position
            GameManagerConnect.Instance.GetCurrentChip().transform.DOMove(position, 0.125f);
        }
    }
    private void GM_ColumnSelect(int x, int y, GameManagerConnect.PlayerType playerType)
    {
        StackChipOnColumnRpc(x, y, playerType);
    }

    [Rpc(SendTo.Server)]
    private void SpawnChipRpc(Vector3 position, GameManagerConnect.PlayerType playerType)
    {
        GameChip chip = Instantiate(chipPrefab, position, Quaternion.identity);
        chip.GetComponent<NetworkObject>().Spawn(true);

        GameManagerConnect.Instance.SetCurrentChip(chip);

        visualGameObjects.Add(chip.gameObject);

        // hopefully network spawn handles this
        //ChangeChipColorRpc(playerType);
    }
    [Rpc(SendTo.Server)]
    private void StackChipOnColumnRpc(int x, int y, GameManagerConnect.PlayerType playerType)
    {
        // move chip
        Vector3 position = new Vector3(x, y, 0);
        GameManagerConnect.Instance.GetCurrentChip().transform.DOMove(position, 0.5f).OnComplete(() =>
        {
            // set waiting to false
            GameManagerConnect.Instance.waitingMovement.Value = false;

            // check winner
            GameManagerConnect.Instance.CheckWinnerRpc(playerType);
        });

        // set current chip to null
        GameManagerConnect.Instance.SetCurrentChip(null);

    }

    private Vector3 GetGridWorldPosition(int x, int y)
    {
        return new Vector3(-GRID_WIDTH + x * GRID_WIDTH,-GRID_HEIGHT + y * GRID_HEIGHT, 0);
    }
}
