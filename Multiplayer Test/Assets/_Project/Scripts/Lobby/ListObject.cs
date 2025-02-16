using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class ListObject : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI playerText;
    [SerializeField] private Button joinButton;

    private Lobby savedLobby;

    private void Awake()
    {
        if (joinButton != null) joinButton.onClick.AddListener(() =>
        {
            Join();
        });
    }
    public void SetData(Lobby lobby)
    {
        savedLobby = lobby;

        if (nameText != null) nameText.text = "Lobby " + lobby.Data[QuickLobby.CODE_KEY].Value;
        if (playerText != null) playerText.text = lobby.Players.Count.ToString() + "/" + lobby.MaxPlayers.ToString();

    }
    private void Join()
    {
        if (savedLobby == null) return;

        QuickLobby.Instance.JoinLobbyById(savedLobby);
    }
}
