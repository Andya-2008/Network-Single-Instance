using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyItem : MonoBehaviour
{
    [SerializeField] private TMP_Text lobbyNameText;
    //[SerializeField] private TMP_Text lobbyPlayersText;

    public const string JoinCodeKey = "JoinCode";

    private LobbiesList lobbiesList;
    private Lobby lobby;

    public void Initiatilize(LobbiesList lobbiesList, Lobby lobby) {
        this.lobbiesList = lobbiesList;
        this.lobby = lobby;
        lobbyNameText.text = lobby.Name;
        //lobbyPlayersText.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";

    }


    public void Join()
    {

        MainMenu mm = GameObject.Find("MainMenu").GetComponent<MainMenu>();
        mm.saveName();
        lobbiesList.StopAllCoroutines();
        Debug.Log("Joining");
        lobbiesList.JoinAsync(lobby);
        
    }
}
