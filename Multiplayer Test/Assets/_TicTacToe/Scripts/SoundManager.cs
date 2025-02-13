using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private Transform placeSfxPrefab;
    [SerializeField] private Transform winSfxPrefab;
    [SerializeField] private Transform loseSfxPrefab;

    private void Start()
    {
        GameManager.Instance.OnPlaceObject += GM_ObjectPlaced;
        GameManager.Instance.OnGameWin += GM_GameWin;
    }
    private void OnDisable()
    {
        GameManager.Instance.OnPlaceObject -= GM_ObjectPlaced;
        GameManager.Instance.OnGameWin -= GM_GameWin;
    }

    private void GM_GameWin(GameManager.Line line, GameManager.PlayerType winPlayerType)
    {
        if (GameManager.Instance.localPlayerType == winPlayerType)
        {
            Transform sfxObject = Instantiate(winSfxPrefab);
            Destroy(sfxObject.gameObject, 5f);
        }
        else
        {
            Transform sfxObject = Instantiate(loseSfxPrefab);
            Destroy(sfxObject.gameObject, 5f);
        }
    }
    private void GM_ObjectPlaced()
    {
        Transform sfxObject = Instantiate(placeSfxPrefab);
        Destroy(sfxObject.gameObject, 5f);
    }
}
