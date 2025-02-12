using DG.Tweening;
using Unity.Netcode;
using UnityEngine;

public class GameVisualManager : NetworkBehaviour
{
    private const float GRID_SIZE = 1f;

    [Header("Prefabs")]
    [SerializeField] private Transform crossPrefab;
    [SerializeField] private Transform circlePrefab;


    private void Start()
    {
        GameManager.Instance.OnClickedOnGridPosition += ClickedOnGridPosition;
    }
    private void OnDisable()
    {
        GameManager.Instance.OnClickedOnGridPosition -= ClickedOnGridPosition;
    }

    private void ClickedOnGridPosition(int x, int y, GameManager.PlayerType playerType)
    {
        Debug.Log("ClickedEvent");

        SpawnObjectRpc(x, y, playerType);
    }

    [Rpc(SendTo.Server)]
    private void SpawnObjectRpc(int x, int y, GameManager.PlayerType playerType)
    {
        Debug.Log("SpawnRpc");

        Transform prefab;
        if (playerType == GameManager.PlayerType.Cross) prefab = crossPrefab;
        else prefab = circlePrefab;

        Transform spawnedCrossTransform = Instantiate(prefab, GetGridWorldPosition(x, y), Quaternion.identity);
        spawnedCrossTransform.GetComponent<NetworkObject>().Spawn(true);
        //spawnedCrossTransform.position = GetGridWorldPosition(x, y);
    }

    private Vector3 GetGridWorldPosition(int x, int y)
    {
        return new Vector3(-GRID_SIZE + x * GRID_SIZE, 0, -GRID_SIZE + y * GRID_SIZE);
    }
}


