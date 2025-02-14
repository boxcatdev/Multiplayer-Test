using TMPro;
using UnityEngine;

public class ConnectUI : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private GameObject redHighlight;
    [SerializeField] private GameObject yellowHighlight;
    [Space]
    [SerializeField] private GameObject redYouText;
    [SerializeField] private GameObject yellowYouText;
    [Space]
    [SerializeField] private TextMeshProUGUI redScoreText;
    [SerializeField] private TextMeshProUGUI yellowScoreText;

    private void Awake()
    {
        if (redHighlight != null) redHighlight.SetActive(false);
        if (yellowHighlight != null) yellowHighlight.SetActive(false);
        if (redYouText != null) redYouText.SetActive(false);
        if (yellowYouText != null) yellowYouText.SetActive(false);

        if (redScoreText != null) redScoreText.text = "";
        if (yellowScoreText != null) yellowScoreText.text = "";
    }
    private void Start()
    {
        GameManagerConnect.Instance.OnScoreValueChanged += GM_ScoreChanged;
        GameManagerConnect.Instance.OnGameStart += GM_GameStarted;
        GameManagerConnect.Instance.OnCurrentTurnValueChanged += UpdateHighlight;
    }

    private void GM_ScoreChanged()
    {
        if (redScoreText != null) redScoreText.text = GameManagerConnect.Instance.playerRedScore.Value.ToString();
        if (yellowScoreText != null) yellowScoreText.text = GameManagerConnect.Instance.playerYellowScore.Value.ToString();
    }
    private void GM_GameStarted()
    {
        if (GameManagerConnect.Instance.GetLocalPlayerType() == GameManagerConnect.PlayerType.Red)
        {
            if (redYouText != null) redYouText.SetActive(true);
        }
        else
        {
            if (yellowYouText != null) yellowYouText.SetActive(true);
        }

        if (redScoreText != null) redScoreText.text = "0";
        if (yellowScoreText != null) yellowScoreText.text = "0";

        UpdateHighlight();
    }

    private void UpdateHighlight()
    {
        if (GameManagerConnect.Instance.currentPlayerTurn.Value == GameManagerConnect.PlayerType.Red)
        {
            if (redHighlight != null) redHighlight.SetActive(true);
            if (yellowHighlight != null) yellowHighlight.SetActive(false);
        }
        else if (GameManagerConnect.Instance.currentPlayerTurn.Value == GameManagerConnect.PlayerType.Yellow)
        {
            if (redHighlight != null) redHighlight.SetActive(false);
            if (yellowHighlight != null) yellowHighlight.SetActive(true);
        }
        else
        {
            if (redHighlight != null) redHighlight.SetActive(false);
            if (yellowHighlight != null) yellowHighlight.SetActive(false);
        }
    }
}
