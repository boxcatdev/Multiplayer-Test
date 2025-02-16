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

    public static QuickLobby Instance;

    public const string CODE_KEY = "join";
    public const string LOBBY_NAME = "Default Lobby";

    [Header("Join")]
    [SerializeField] private GameObject lobbyUI;
    [Space]
    [SerializeField] private TextMeshProUGUI joiningText;
    [SerializeField] private Button joinButton;

    //[SerializeField] private TextMeshProUGUI lobbylistText;

    [Header("Game")]
    [SerializeField] private TextMeshProUGUI lobbyCodeText;
    [SerializeField] private GameObject lobbyCodeContainer;


    [Header("List")]
    [SerializeField] private Button refreshButton;
    [Space]
    [SerializeField] private List<ListObject> listObjects = new List<ListObject>();

    private Lobby savedLobby;

    public Action<Lobby> OnJoinedGame = delegate { };

    private void Awake()
    {
        // singleton
        Instance = this;

        joinButton.onClick.AddListener(() =>
        {
            TryJoinOrCreate();
        });
        refreshButton.onClick.AddListener(() =>
        {
            ListLobbies();
        });
    }
    private void OnEnable()
    {
        OnJoinedGame += QL_OnJoin;
    }
    private void OnDisable()
    {
        OnJoinedGame -= QL_OnJoin;
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

        // start list lobbies
        ListLobbies();
    }

    private void QL_OnJoin(Lobby lobby)
    {

        if (lobby != null)
        {
            lobbyUI.gameObject.SetActive(false);
        }
        else
        {
            if (joiningText != null) joiningText.text = "Failed to find lobby";
            lobbyUI.SetActive(true);
        }
    }

    private async void TryJoinOrCreate()
    {
        // get lobby count first then try
        QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(GetQueryLobbiesOptions());

        if (queryResponse.Results.Count == 0)
        {
            Lobby lobby = await CreateLobby();
            OnJoinedGame?.Invoke(lobby);
        }
        else 
        {
            Lobby lobby = await QuickJoinLobby();
            OnJoinedGame?.Invoke(lobby);
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

            ShowLobbyCode(lobby.Data[CODE_KEY].Value);

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
    public async void JoinLobbyById(Lobby lobby)
    {
        try
        {
            Debug.Log("Join by ID");

            Lobby joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id);

            if (joiningText != null) joiningText.text = joiningText.text + "\n" + lobby.Data[CODE_KEY].Value;

            // continues if lobby is found
            savedLobby = lobby;
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(lobby.Data[CODE_KEY].Value);

            ShowLobbyCode(lobby.Data[CODE_KEY].Value);

            // start client
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData);

            NetworkManager.Singleton.StartClient();

            OnJoinedGame?.Invoke(joinedLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogWarning(e);
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

            ShowLobbyCode(lobby.Data[CODE_KEY].Value);

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
            if (listObjects.Count > 0)
            {
                QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions()
                {
                    Count = 3,
                    Filters = new List<QueryFilter>
                    {
                        new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                    }
                };

                QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(GetQueryLobbiesOptions());

                for (int i = 0; i < listObjects.Count; i++)
                {
                    if (i < queryResponse.Results.Count)
                    {
                        // enable
                        listObjects[i].gameObject.SetActive(true);
                        listObjects[i].SetData(queryResponse.Results[i]);
                    }
                    else
                    {
                        // disable
                        listObjects[i].gameObject.SetActive(false);
                    }
                }

                StartCoroutine(DisableRefresh(2f));
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.LogWarning(e);
        }
    }

    private void ShowLobbyCode(string code)
    {
        if (lobbyCodeText != null && lobbyCodeContainer != null)
        {
            lobbyCodeContainer.SetActive(true);
            //lobbyCodeText.gameObject.SetActive(true);
            lobbyCodeText.text = "Lobby " + code;
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
    private IEnumerator DisableRefresh(float delay)
    {
        refreshButton.enabled = false;

        yield return new WaitForSeconds(delay);

        refreshButton.enabled = true;
    }
}
