using DG.Tweening;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameVisualManager : NetworkBehaviour
{
    private const float GRID_SIZE = 1f;

    [Header("Prefabs")]
    [SerializeField] private Transform crossPrefab;
    [SerializeField] private Transform circlePrefab;
    [SerializeField] private Transform lineCompletePrefab;

    private List<GameObject> visualGameObjects;

    private void Awake()
    {
        visualGameObjects = new List<GameObject>();
    }
    private void Start()
    {
        GameManager.Instance.OnClickedOnGridPosition += GM_ClickedOnGridPosition;
        GameManager.Instance.OnGameWin += GM_GameWin;
        GameManager.Instance.OnRematch += GM_Rematch;
    }
    private void OnDisable()
    {
        GameManager.Instance.OnClickedOnGridPosition -= GM_ClickedOnGridPosition;
        GameManager.Instance.OnGameWin -= GM_GameWin;
        GameManager.Instance.OnRematch -= GM_Rematch;
    }

    private void GM_Rematch()
    {
        foreach (GameObject gameObject in visualGameObjects)
        {
            Destroy(gameObject);
        }
        visualGameObjects.Clear();
    }
    private void GM_GameWin(GameManager.Line line, GameManager.PlayerType playerType)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        float eulerY = 0f;
        switch (line.orientation)
        {
            default:
                eulerY = 0f;
                break;
            case GameManager.Orientation.Horizontal:
                eulerY = 0f;
                break;
            case GameManager.Orientation.Vertical:
                eulerY = 90f;
                break;
            case GameManager.Orientation.DiagonalA:
                eulerY = 45f;
                break;
            case GameManager.Orientation.DiagonalB:
                eulerY = -45f;
                break;
        }

        Transform lineCompleteTransform = Instantiate(lineCompletePrefab, GetGridWorldPosition(line.centerGridPosition.x, line.centerGridPosition.y), Quaternion.Euler(0, eulerY, 0));
        lineCompleteTransform.GetComponent<NetworkObject>().Spawn(true);

        visualGameObjects.Add(lineCompleteTransform.gameObject);
    }
    private void GM_ClickedOnGridPosition(int x, int y, GameManager.PlayerType playerType)
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

        visualGameObjects.Add(spawnedCrossTransform.gameObject);
    }

    private Vector3 GetGridWorldPosition(int x, int y)
    {
        return new Vector3(-GRID_SIZE + x * GRID_SIZE, 0, -GRID_SIZE + y * GRID_SIZE);
    }
}


