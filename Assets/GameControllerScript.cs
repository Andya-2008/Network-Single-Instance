using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using UnityEngine;
using System.Linq;

public class GameControllerScript : NetworkBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text orderListText;
    [SerializeField] private GameObject clearButton;

    public const string PlayerNameKey = "PlayerName";
    public const string JoinCodeKey = "JoinCode";

    private string name;
    private Dictionary<ulong, string> buzzList = new Dictionary<ulong, string>();
    private Dictionary<ulong, float> orderList = new Dictionary<ulong, float>();
    private Dictionary<ulong, float> latencyList = new Dictionary<ulong, float>();
    private Dictionary<ulong, string> soundList = new Dictionary<ulong, string>();
    private Queue<float> latencyQueue = new Queue<float>();


    private List<ulong> lstAlreadyBuzzed = new List<ulong>();
    // Start is called before the first frame update
    async void Start()
    {
        name = PlayerPrefs.GetString(PlayerNameKey, string.Empty);
        nameText.text = name;

        string hostName = ClientSingleton.Instance.GameManager.joinedHostName;
        nameText.text = name + " connected to " + hostName;

        if (NetworkManager.IsServer) {
            clearButton.SetActive(true);
        }

        // Calibrate Latency (3 times) and register the player name
        for (int i = 0; i < 3; i++)
        {
            registerServerRpc(name, Time.time);
        }
    }

    // When the button is pressed, call server RPC
    public void PushButton() {
        buzzServerRpc(Time.time);

    }

    // Clears the buzzer List
    public void ClearList() {
        lstAlreadyBuzzed.Clear();
        orderList.Clear();
        PublishToClientRpc(NetworkManager.ServerClientId, -1.0f, "");

    }


    //  use this server registration RPC to register client name and also 
    //  calibrate client latency
    [ServerRpc(RequireOwnership = false)]
    private void registerServerRpc(string playerName, float buzzTime, ServerRpcParams serverRpcParams = default) {
        ulong clientId = serverRpcParams.Receive.SenderClientId;
        if (!buzzList.ContainsKey(clientId))
        {
            buzzList.Add(clientId, playerName);
        }
        PublishToClientRpc(clientId, buzzTime, "");
    }


    // This is what happens on Server when a buzzer is pressed.
    [ServerRpc(RequireOwnership = false)]
    private void buzzServerRpc(float buzzTime, ServerRpcParams serverRpcParams = default) {
        
        ulong clientId = serverRpcParams.Receive.SenderClientId;
        if (!lstAlreadyBuzzed.Contains(clientId))
        {
            lstAlreadyBuzzed.Add(clientId);


            // just in case the latency list is not populated.
            if (!latencyList.ContainsKey(clientId))
            {
                latencyList.Add(clientId, 0f);
            }

            // order List is the list in which the users buzzed in.
            orderList.Add(clientId, Time.time-latencyList[clientId]);

            

            string strList = "";

            // create a new list in order of users that buzzed in.
            var orderedList = orderList.OrderBy(pair => pair.Value).Take(orderList.Count);

            int i=1;
            // creates a string with a list of all users that buzzed in.
            foreach (KeyValuePair<ulong, float> entry in orderedList)
            {
                /*
                Debug.Log(entry.Key);
                Debug.Log(orderList[entry.Key]);
                Debug.Log(buzzList[entry.Key]);
                */
                strList = strList + "\n" + i + ". " + buzzList[entry.Key] + ":" + latencyList[entry.Key].ToString("0.000") + ":" + orderList[entry.Key].ToString("0.000") ;
                i++;
            }
            //orderListText.text = strList;

            // publish the buzzed in list to all clients.
            PublishToClientRpc(clientId, buzzTime, strList);
            AudioSource audio = this.GetComponent<AudioSource>();
            string audioPath = "sounds/" + soundList[clientId];
            AudioClip clip = (AudioClip)Resources.Load(audioPath);
            audio.PlayOneShot(clip);
        }

    }

    //  Register Sound
    [ServerRpc(RequireOwnership = false)]
    public void registerSoundServerRpc(string sound, ServerRpcParams serverRpcParams = default) {
        ulong clientId = serverRpcParams.Receive.SenderClientId;
        soundList[clientId] = sound;

    }
    
    
    // record the latency for each client.
    [ServerRpc(RequireOwnership = false)]
    private void latencyServerRpc(float latency, ServerRpcParams serverRpcParams = default)
    {
        ulong clientId = serverRpcParams.Receive.SenderClientId;
        latencyList[clientId] = latency;
    }
    
    // publish the list of all people who buzzed in.
    // this is also used to handle the latency for each buzz.
    [ClientRpc]
    private void PublishToClientRpc(ulong clientId, float buzzTime, string strList) {
        
        // publish the list of people that buzzed in on each client.
        orderListText.text = strList;

        //  If the buzz came from this client, then see how long it took to do a round trip.
        //  Put the round trip latency in the latencyQueue.  Only keep the last 5.
        if (NetworkManager.LocalClientId == clientId && buzzTime >=0) {
            latencyQueue.Enqueue(Time.time - buzzTime);
            if (latencyQueue.Count > 5)
            {
                latencyQueue.Dequeue();
            }
        }

        // take average of the latency queue.
        float total = 0;
        foreach (float l in latencyQueue) {
            total += l;
        }
        float avgLatency = total / latencyQueue.Count;
        
        //  One way latency is the round trip latency divided by 2.
        avgLatency = avgLatency / 2;

        // submit the avg latency to the server.
        latencyServerRpc(avgLatency);
        
    }

    private void OnApplicationQuit()
    {
        if (HostSingleton.Instance.GameManager != null)
        {
            HostSingleton.Instance.GameManager.Dispose();
        }
    }

}
