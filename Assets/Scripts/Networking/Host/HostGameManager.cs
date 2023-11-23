using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Unity.Services.Relay;
using System;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Text;
using Unity.Services.Authentication;

public class HostGameManager : IDisposable
{
    public Allocation allocation;
    public string joinedHostName;
    private string joinCode;
    private string lobbyId;
    private const int MaxConnections = 20;
    private const string GameSceneName = "Game";
    private NetworkServer networkServer;
    public async Task StartHostAsync() {
        try
        {
            allocation = await Relay.Instance.CreateAllocationAsync(MaxConnections);
        } catch (Exception ex) {
            Debug.Log(ex);
            return;
        }


        try
        {
            joinCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log(joinCode);
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
            return;
        }

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
        transport.SetRelayServerData(relayServerData);

        try {
            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();
            lobbyOptions.IsPrivate = false;
            lobbyOptions.Data = new Dictionary<string, DataObject>()
            {
                { 
                    "JoinCode", new DataObject(
                            visibility: DataObject.VisibilityOptions.Member,
                            value: joinCode
                        )
                }
            };

            string playerName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "Missing Name");

            Lobby lobby = await Lobbies.Instance.CreateLobbyAsync($"{playerName}", MaxConnections, lobbyOptions);
            lobbyId = lobby.Id;

            HostSingleton.Instance.StartCoroutine(HeartbeatLobby(15));
            HostSingleton.Instance.GameManager.joinedHostName = lobby.Name;
            ClientSingleton.Instance.GameManager.joinedHostName = lobby.Name;
        }
        catch (LobbyServiceException ex) {
            Debug.Log(ex);
            return;
        }

        networkServer = new NetworkServer(NetworkManager.Singleton);

        UserData userData = new UserData
        {
            userName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "Missing Name"),
            userAuthId = AuthenticationService.Instance.PlayerId
        };

        string payload = JsonUtility.ToJson(userData);
        byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);

        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;





        NetworkManager.Singleton.StartHost();

        NetworkManager.Singleton.SceneManager.LoadScene(GameSceneName, LoadSceneMode.Single);
    }

    private IEnumerator HeartbeatLobby(float waitTimeSeconds) {
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(waitTimeSeconds);
        while (true) {
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }

    public async void Dispose()
    {
        HostSingleton.Instance.StopCoroutine(nameof(HeartbeatLobby));

        if (!string.IsNullOrEmpty(lobbyId))
        {
            try
            {
                await Lobbies.Instance.DeleteLobbyAsync(lobbyId);
            }
            catch (LobbyServiceException ex) {
                Debug.Log(ex);

            
            }

            lobbyId = string.Empty;

        }

        networkServer?.Dispose();
    }

}
