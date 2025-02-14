using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class GameOverConnectUI : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private GameObject backgroundPanel;
    [SerializeField] private TextMeshProUGUI resultsText;
    [SerializeField] private Color winColor = Color.white;
    [SerializeField] private Color loseColor = Color.white;
    [SerializeField] private Color tieColor = Color.white;
    [Space]
    [SerializeField] private Button rematchButton;

    private void Awake()
    {
        rematchButton.onClick.AddListener(() =>
        {
            GameManagerConnect.Instance.RematchGameRpc();
        });
    }
    private void Start()
    {
        GameManagerConnect.Instance.OnGameWin += GM_GameWin;
        GameManagerConnect.Instance.OnGameDraw += GM_GameDraw;
        GameManagerConnect.Instance.OnGameRematch += GM_GameRematch;

        Hide();
    }
    private void OnDisable()
    {
        GameManagerConnect.Instance.OnGameWin -= GM_GameWin;
        GameManagerConnect.Instance.OnGameDraw -= GM_GameDraw;
        GameManagerConnect.Instance.OnGameRematch -= GM_GameRematch;
    }

    private void GM_GameWin(GameManagerConnect.PlayerType winningPlayerType)
    {
        Debug.LogWarning("GameOverUI: Win!");

        if (resultsText != null) resultsText.text = winningPlayerType.ToString() + " Wins!";
        
        if (winningPlayerType == GameManagerConnect.Instance.GetLocalPlayerType())
        {
            if (resultsText != null) resultsText.text = "You Win!";
            if (resultsText != null) resultsText.color = winColor;
        }
        else
        {
            if (resultsText != null) resultsText.text = "You Lose!";
            if (resultsText != null) resultsText.color = loseColor;
        }

        Show();
    }
    private void GM_GameDraw()
    {
        Debug.LogWarning("GameOverUI: Draw!");

        if (resultsText != null) resultsText.text = "Draw!";
        
        Show();
    }
    private void GM_GameRematch()
    {
        Hide();
    }

    private void Show()
    {
        backgroundPanel.SetActive(true);
    }
    private void Hide()
    {
        backgroundPanel.SetActive(false);
    }
}
