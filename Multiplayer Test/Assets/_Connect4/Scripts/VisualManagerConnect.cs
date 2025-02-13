using Unity.Netcode;
using UnityEngine;

public class VisualManagerConnect : NetworkBehaviour
{
    public const int GRID_WIDTH = 7;
    public const int GRID_HEIGHT = 6;

    private float yOffset = 6.5f;

    [Header("Prefabs")]
    [SerializeField] private GameChip chipPrefab;

    [Header("Materials")]
    [SerializeField] private Material redMaterial;
    [SerializeField] private Material yellowMaterial;


    private void Start()
    {
        GameManagerConnect.Instance.OnColumnHover += GM_ColumnHover;
    }



    private void GM_ColumnHover(GridColumn column, GameManagerConnect.PlayerType playerType)
    {
        Vector3 position = column.transform.position + (Vector3.up * yOffset);

        if (GameManagerConnect.Instance.GetCurrentChip() == null)
        {
            // spawn chip
            SpawnChipRpc(position, playerType);
        }
        else
        {
            // move to column position
            GameManagerConnect.Instance.GetCurrentChip().transform.position = position;
        }
    }

    [Rpc(SendTo.Server)]
    private void SpawnChipRpc(Vector3 position, GameManagerConnect.PlayerType playerType)
    {
        GameChip chip = Instantiate(chipPrefab, position, Quaternion.identity);
        chip.ChangeMaterial(playerType == GameManagerConnect.PlayerType.Red ? redMaterial : yellowMaterial);
        chip.GetComponent<NetworkObject>().Spawn(true);

        GameManagerConnect.Instance.SetCurrentChip(chip);
    }


    private Vector3 GetGridWorldPosition(int x, int y)
    {
        return new Vector3(-GRID_WIDTH + x * GRID_WIDTH, 0, -GRID_HEIGHT + y * GRID_HEIGHT);
    }
}
