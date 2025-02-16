using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class QuickLobby : MonoBehaviour
{
    /// if no lobbies exist then create new one
    ///     do relay and save join code to lobby data
    ///     start host
    /// quick join existing lobby
    ///     connect to relay using saved data join code
    ///     start client
    /// after 2 players joined
    ///     start game
    ///     

    private const string CODE_KEY = "join";
    private const string LOBBY_NAME = "Default Lobby";

    [SerializeField] private GameObject lobbyUI;
    [Space]
    [SerializeField] private Button joinButton;
    [SerializeField] private Button refreshButton;
    [SerializeField] private TextMeshProUGUI lobbyCodeText;
    [SerializeField] private TextMeshProUGUI lobbylistText;
    [SerializeField] private TextMeshProUGUI joiningText;

    private Lobby savedLobby;

    private void Awake()
    {
        joinButton.onClick.AddListener(() =>
        {
            TryJoinOrCreate();
        });
        refreshButton.onClick.AddListener(() =>
        {
            ListLobbies();
        });
    }
    private void Start()
    {
        Authentication();
    }

    private async void Authentication()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private async void TryJoinOrCreate()
    {
        // get lobby count first then try
        QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(GetQueryLobbiesOptions());

        if (queryResponse.Results.Count == 0)
        {
            Lobby lobby = await CreateLobby();
            if (lobby != null) lobbyUI.gameObject.SetActive(false);
        }
        else 
        {
            Lobby lobby = await QuickJoinLobby();
            if (lobby != null) lobbyUI.gameObject.SetActive(false);
            else 
            { 
                if (joiningText != null) joiningText.text = "Failed to find lobby";
                lobbyUI.SetActive(true);
            }
        }

        //Lobby lobby = await QuickJoinLobby() ?? await CreateLobby();

        //if (lobby != null) lobbyUI.gameObject.SetActive(false);
    }

    private async Task<Lobby> QuickJoinLobby()
    {
        try
        {
            Debug.Log("Try Join");

            // look for lobby
            Lobby lobby = await LobbyService.Instance.QuickJoinLobbyAsync();

            if (joiningText != null) joiningText.text = joiningText.text + "\n" + lobby.Data[CODE_KEY].Value;

            // continues if lobby is found
            savedLobby = lobby;
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(lobby.Data[CODE_KEY].Value);

            if (lobbyCodeText != null)
            {
                lobbyCodeText.gameObject.SetActive(true);
                lobbyCodeText.text = "Code: " + lobby.Data[CODE_KEY].Value;
            }

            // start client
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData);

            NetworkManager.Singleton.StartClient();

            return lobby;
        }
        catch
        {
            Debug.Log("No lobby found");
            return null;
        }
    }
    private async Task<Lobby> CreateLobby()
    {
        try
        {
            Debug.Log("Try create lobby");

            int maxPlayers = 2;

            // start relay
            var allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
            var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            // add join code data
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions()
            {
                Data = new Dictionary<string, DataObject> { { CODE_KEY, new DataObject(DataObject.VisibilityOptions.Public, joinCode) } }
            };

            // create lobby
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(LOBBY_NAME, maxPlayers, createLobbyOptions);

            if (lobbyCodeText != null)
            {
                lobbyCodeText.gameObject.SetActive(true);
                lobbyCodeText.text = "Code: " + lobby.Data[CODE_KEY].Value;
            }

            // heartbeat
            StartCoroutine(SendHeartbeat(10.5f, lobby));

            // start host
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData);

            NetworkManager.Singleton.StartHost();

            return lobby;
        }
        catch (LobbyServiceException e)
        {
            Debug.LogWarning(e);
            return null;
        }
    }

    private async void ListLobbies()
    {
        try
        {
            if (lobbylistText != null) lobbylistText.text = "";

            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(GetQueryLobbiesOptions());

            int index = 1;
            foreach (var result in queryResponse.Results)
            {
                if (lobbylistText != null) lobbylistText.text = GetLobbyText(result, index);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    private string GetLobbyText(Lobby lobby, int index) 
    {
        return "Lobby (" + index + ") " + lobby.Data[CODE_KEY].Value + " " + lobby.Players.Count + "/" + lobby.MaxPlayers + "\n";
    }
    private QueryLobbiesOptions GetQueryLobbiesOptions()
    {
        return new QueryLobbiesOptions()
            {
                Count = 10,
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                }
            };
    }
    private IEnumerator SendHeartbeat(float delay, Lobby lobby)
    {
        yield return new WaitForSeconds(delay);

        LobbyService.Instance.SendHeartbeatPingAsync(lobby.Id);

    }
}
