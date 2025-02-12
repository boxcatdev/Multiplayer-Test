using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private GameObject crossArrowGameObject;
    [SerializeField] private GameObject circleArrowGameObject;
    [SerializeField] private GameObject crossYouTextGameObject;
    [SerializeField] private GameObject circleYouTextGameObject;

    private void Awake()
    {
        if (crossArrowGameObject != null) crossArrowGameObject.SetActive(false);
        if (circleArrowGameObject != null) circleArrowGameObject.SetActive(false);
        if (crossYouTextGameObject != null) crossYouTextGameObject.SetActive(false);
        if (circleYouTextGameObject != null) circleYouTextGameObject.SetActive(false);
    }
    private void Start()
    {
        GameManager.Instance.OnGameStarted += GameStarted;
        GameManager.Instance.OnCurrentPlayerTurnChanged += UpdateCurrentArrow;
    }
    private void OnDisable()
    {
        GameManager.Instance.OnGameStarted -= GameStarted;
        GameManager.Instance.OnCurrentPlayerTurnChanged -= UpdateCurrentArrow;
    }

    private void GameStarted()
    {
        if (GameManager.Instance.localPlayerType == GameManager.PlayerType.Cross)
        {
            if (crossYouTextGameObject != null) crossYouTextGameObject.SetActive(true);
        }
        else
        {
            if (circleYouTextGameObject != null) circleYouTextGameObject.SetActive(true);
        }

        UpdateCurrentArrow();
    }

    private void UpdateCurrentArrow()
    {
        if (GameManager.Instance.currentTurnPlayerType.Value == GameManager.PlayerType.Cross)
        {
            if (crossArrowGameObject != null) crossArrowGameObject.SetActive(true);
            if (circleArrowGameObject != null) circleArrowGameObject.SetActive(false);
        }
        else
        {
            if (crossArrowGameObject != null) crossArrowGameObject.SetActive(false);
            if (circleArrowGameObject != null) circleArrowGameObject.SetActive(true);
        }
    }
}
