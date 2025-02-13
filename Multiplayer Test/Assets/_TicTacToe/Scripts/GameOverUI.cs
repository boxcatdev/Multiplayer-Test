using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [Header("Dependencies")]
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
            GameManager.Instance.RematchRpc();
        });
    }
    private void Start()
    {
        GameManager.Instance.OnGameWin += GM_GameWin;
        GameManager.Instance.OnRematch += GM_Rematch;
        GameManager.Instance.OnGameTied += GM_GameTied;

        Hide();
    }

    private void GM_GameTied()
    {
        resultsText.text = "Tie!";
        resultsText.color = tieColor;
        Show();
    }
    private void GM_Rematch()
    {
        Hide();
    }
    private void GM_GameWin(GameManager.Line line, GameManager.PlayerType playerType)
    {
        if (playerType == GameManager.Instance.localPlayerType)
        {
            resultsText.text = "You Win!";
            resultsText.color = winColor;
        }
        else
        {
            resultsText.text = "You Lose!";
            resultsText.color = loseColor;
        }
        Show();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
