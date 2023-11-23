using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbiesList : MonoBehaviour
{
    [SerializeField] private LobbyItem lobbyItemPrefab;
    [SerializeField] private Transform lobbyItemParent;


    private bool isJoining = false;
    private bool isRefreshing = false;

    private void OnEnable()
    {
        RefreshList();
        StartCoroutine(RefreshLobbyList(10));
    }

    public async void RefreshList() {
        if (isRefreshing) return;
        isRefreshing = true;

        QueryLobbiesOptions options = new QueryLobbiesOptions();
        try
        {
            
            options.Count = 25;
            options.Filters = new List<QueryFilter>()
            {
                new QueryFilter(field: QueryFilter.FieldOptions.AvailableSlots,
                                op: QueryFilter.OpOptions.GT,
                                value: "0"),
                new QueryFilter(field: QueryFilter.FieldOptions.IsLocked,
                                op: QueryFilter.OpOptions.EQ,
                                value: "0")

            };
        }
        catch (LobbyServiceException ex) {
            Debug.Log(ex);
        
        }
        QueryResponse lobbies = await Lobbies.Instance.QueryLobbiesAsync(options);

        foreach (Transform child in lobbyItemParent) {
            Destroy(child.gameObject);
        }
        foreach (Lobby lobby in lobbies.Results) {
            LobbyItem lobbyItem = Instantiate(lobbyItemPrefab, lobbyItemParent);
            lobbyItem.Initiatilize(this, lobby);
        }
        isRefreshing = false;
    }

    public async Task JoinAsync(Lobby lobby)
    {
        Debug.Log("JoinAsync:" + lobby.Id + ":" + isJoining);
        if (isJoining) return;
        isJoining = true;
        try {

            Lobby joiningLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobby.Id);
            Debug.Log("joiningLobby:" + joiningLobby.Id);
            string joinCode = joiningLobby.Data["JoinCode"].Value;
            Debug.Log("joincode:" + joinCode);
            ClientSingleton.Instance.GameManager.StartClientAsync(joinCode);
            ClientSingleton.Instance.GameManager.joinedHostName = lobby.Name;
            

        } catch (LobbyServiceException ex) {
            Debug.Log(ex);
           
        }
        isJoining = false;
    }

    private IEnumerator RefreshLobbyList(float waitTimeSeconds)
    {
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(waitTimeSeconds);
        while (true)
        {
            RefreshList();
            yield return delay;
        }
    }
}
