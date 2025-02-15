using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class TestLobby : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button _createButton;
    [SerializeField] private Button _listButton;
    [SerializeField] private Button _quickJoinButton;

    private Lobby hostLobby;
    private Lobby joinedLobby;

    private float heartbeatTimer = 15f;
    private float heartbeatProgress;

    private float pollTimer = 1.1f;
    private float pollProgress;

    private string playerName;

    private void Awake()
    {
        if (_createButton != null) _createButton.onClick.AddListener(() =>
        {
            CreatePublicLobby();
        });
        if (_listButton != null) _listButton.onClick.AddListener(() =>
        {
            ListLobbies();
        });
        if (_quickJoinButton != null) _quickJoinButton.onClick.AddListener(() =>
        {
            QuickJoinLobby();
        });
    }
    private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        playerName = "Player " + Random.Range(1, 99);
    }
    private void Update()
    {
        HandleHeartbeat();
        HandleLobbyPollForUpdates();
    }

    private async void HandleHeartbeat()
    {
        if (hostLobby != null)
        {
            heartbeatProgress -= Time.deltaTime;
            if (heartbeatProgress <= 0)
            {
                heartbeatProgress = heartbeatTimer;

                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }
    }
    private async void HandleLobbyPollForUpdates()
    {
        if (joinedLobby != null)
        {
            pollProgress -= Time.deltaTime;
            if (pollProgress <= 0)
            {
                pollProgress = pollTimer;

                await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
            }
        }
    }

    private async void CreatePublicLobby()
    {
        try
        {
            string lobbyName = "Lobby " + Random.Range(1, 99).ToString();
            int maxPlayers = 2;

            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions()
            {
                IsPrivate = false,
                Player = GetPlayer()
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);

            hostLobby = lobby;
            joinedLobby = hostLobby;

            Debug.Log("Created lobby! " + lobbyName + " " + maxPlayers);

            PrintPlayers(hostLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    private async void ListLobbies()
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions()
            {
                Count = 10,
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                }
            };

            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);

            Debug.Log("Lobbies found: " + queryResponse.Results.Count);
            foreach (var result in queryResponse.Results)
            {
                Debug.Log(result.Name + " " + result.MaxPlayers + " " + result.Players.Count);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    private async void JoinLobbyByCode(string lobbyCode)
    {
        try
        {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions()
            {
                Player = GetPlayer()
            };

            Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);

            joinedLobby = lobby;

            Debug.Log("Joined lobby with code: " + lobbyCode);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }
    private async void JoinLobbyByID(Lobby lobby)
    {
        try
        {
            JoinLobbyByIdOptions joinLobbyByIdOptions = new JoinLobbyByIdOptions()
            {
                Player = GetPlayer()
            };

            Lobby jLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id);

            joinedLobby = jLobby;

            Debug.Log("Joined lobby with id: " + lobby.Id);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }
    private async void QuickJoinLobby()
    {
        try
        {
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

            PrintPlayers(joinedLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    private async void LeaveLobby()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);

            Debug.Log($"{playerName} left the lobby");
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    private async void MigrateLobbyHost()
    {
        try
        {
            hostLobby = await LobbyService.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions()
            {
                HostId = hostLobby.Players[1].Id
            });
            joinedLobby = hostLobby;

            PrintPlayers(joinedLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    private Player GetPlayer()
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>()
                    {
                        { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) },
                    }
        };
    }
    private void PrintPlayers(Lobby lobby)
    {
        Debug.Log("Players in lobby " + lobby.Name);
        foreach (var player in lobby.Players)
        {
            Debug.Log(player.Id + " " + player.Data["PlayerName"].Value);
        }
    }
}
