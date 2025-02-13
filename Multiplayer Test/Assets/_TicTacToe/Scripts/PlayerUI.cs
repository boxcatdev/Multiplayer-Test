using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [Header("Player Turn")]
    [SerializeField] private GameObject crossArrowGameObject;
    [SerializeField] private GameObject circleArrowGameObject;
    [SerializeField] private GameObject crossYouTextGameObject;
    [SerializeField] private GameObject circleYouTextGameObject;

    [Header("Score")]
    [SerializeField] private TextMeshProUGUI crossScoreText;
    [SerializeField] private TextMeshProUGUI circleScoreText;

    private void Awake()
    {
        if (crossArrowGameObject != null) crossArrowGameObject.SetActive(false);
        if (circleArrowGameObject != null) circleArrowGameObject.SetActive(false);
        if (crossYouTextGameObject != null) crossYouTextGameObject.SetActive(false);
        if (circleYouTextGameObject != null) circleYouTextGameObject.SetActive(false);

        if (crossScoreText != null) crossScoreText.text = "";
        if (circleScoreText != null) circleScoreText.text = "";
    }
    private void Start()
    {
        GameManager.Instance.OnGameStarted += GM_GameStarted;
        GameManager.Instance.OnCurrentPlayerTurnChanged += UpdateCurrentArrow;
        GameManager.Instance.OnScoreChanged += GM_ScoreChanged;
    }
    private void OnDisable()
    {
        GameManager.Instance.OnGameStarted -= GM_GameStarted;
        GameManager.Instance.OnCurrentPlayerTurnChanged -= UpdateCurrentArrow;
        GameManager.Instance.OnScoreChanged -= GM_ScoreChanged;
    }

    private void GM_ScoreChanged()
    {
        GameManager.Instance.GetScores(out int crossScore, out int circleScore);

        crossScoreText.text = crossScore.ToString();
        circleScoreText.text = circleScore.ToString();
    }
    private void GM_GameStarted()
    {
        if (GameManager.Instance.localPlayerType == GameManager.PlayerType.Cross)
        {
            if (crossYouTextGameObject != null) crossYouTextGameObject.SetActive(true);
        }
        else
        {
            if (circleYouTextGameObject != null) circleYouTextGameObject.SetActive(true);
        }

        crossScoreText.text = "0";
        circleScoreText.text = "0";

        UpdateCurrentArrow();
    }

    private void UpdateCurrentArrow()
    {
        if (GameManager.Instance.currentTurnPlayerType.Value == GameManager.PlayerType.Cross)
        {
            if (crossArrowGameObject != null) crossArrowGameObject.SetActive(true);
            if (circleArrowGameObject != null) circleArrowGameObject.SetActive(false);
        }
        else if (GameManager.Instance.currentTurnPlayerType.Value == GameManager.PlayerType.Circle)
        {
            if (crossArrowGameObject != null) crossArrowGameObject.SetActive(false);
            if (circleArrowGameObject != null) circleArrowGameObject.SetActive(true);
        }
        else
        {
            if (crossArrowGameObject != null) crossArrowGameObject.SetActive(false);
            if (circleArrowGameObject != null) circleArrowGameObject.SetActive(false);
        }
    }
}
