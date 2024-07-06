using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using System.Collections.Generic;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using System.Collections.Concurrent;

public class MatchmakingManager : MonoBehaviour
{
    public Button startButton;

    private const int MaxPlayers = 2;
    private const string LobbyName = "QuickMatchLobby";
    private Lobby currentLobby;
    private string playerId;

    private async void Start()
    {
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        playerId = AuthenticationService.Instance.PlayerId;

        startButton.onClick.AddListener(OnStartButtonClicked);

        InvokeRepeating(nameof(CheckForHostDisconnection), 10, 5); // Check every 5 seconds
    }

    private void OnStartButtonClicked()
    {
        FindOrCreateLobby();
    }

    private async void FindOrCreateLobby()
    {
        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions
            {
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "1", QueryFilter.OpOptions.GE)
                }
            };
            QueryResponse lobbies = await LobbyService.Instance.QueryLobbiesAsync(options);

            if (lobbies.Results.Count > 0)
            {
                string lobbyId = lobbies.Results[0].Id;
                currentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
                Debug.Log("Joined lobby: " + lobbyId);

                await JoinRelayAsync(currentLobby);
            }
            else
            {
                currentLobby = await LobbyService.Instance.CreateLobbyAsync(LobbyName, MaxPlayers);
                Debug.Log("Created new lobby: " + currentLobby.Id);

                await CreateRelayAsync(currentLobby);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    private async System.Threading.Tasks.Task CreateRelayAsync(Lobby lobby)
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(MaxPlayers - 1);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();

            Dictionary<string, DataObject> data = new Dictionary<string, DataObject>
            {
                { "JoinCode", new DataObject(DataObject.VisibilityOptions.Public, joinCode) },
                { "HostId", new DataObject(DataObject.VisibilityOptions.Public, playerId) }
            };
            await LobbyService.Instance.UpdateLobbyAsync(lobby.Id, new UpdateLobbyOptions { Data = data });

            Debug.Log("Relay created with join code: " + joinCode);
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
        }
    }

    private async System.Threading.Tasks.Task JoinRelayAsync(Lobby lobby)
    {
        try
        {
            string joinCode = lobby.Data["JoinCode"].Value;

            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
            Debug.Log("Joined relay with join code: " + joinCode);
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
        }
    }

    private async void CheckForHostDisconnection()
    {
        if (currentLobby != null)
        {
            try
            {
                currentLobby = await LobbyService.Instance.GetLobbyAsync(currentLobby.Id);

                string hostId = currentLobby.Data["HostId"].Value;
                if (!IsPlayerConnected(hostId))
                {
                    await ElectNewHost();
                }
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError(e);
            }
        }
    }

    private bool IsPlayerConnected(string playerId)
    {
        foreach (var player in currentLobby.Players)
        {
            if (player.Id == playerId)
            {
                return true;
            }
        }
        return false;
    }

    private async System.Threading.Tasks.Task ElectNewHost()
    {
        try
        {
            if (currentLobby.Players.Count > 0)
            {
                string newHostId = currentLobby.Players[0].Id;

                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(MaxPlayers - 1);
                string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

                Dictionary<string, DataObject> data = new Dictionary<string, DataObject>
                {
                    { "JoinCode", new DataObject(DataObject.VisibilityOptions.Public, joinCode) },
                    { "HostId", new DataObject(DataObject.VisibilityOptions.Public, newHostId) }
                };
                await LobbyService.Instance.UpdateLobbyAsync(currentLobby.Id, new UpdateLobbyOptions { Data = data });

                Debug.Log("New host elected: " + newHostId);

                if (newHostId == playerId)
                {
                    await JoinRelayAsync(currentLobby);
                }
            }
            else
            {
                await LobbyService.Instance.DeleteLobbyAsync(currentLobby.Id);
                currentLobby = null;
            }
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
        }
    }
}
