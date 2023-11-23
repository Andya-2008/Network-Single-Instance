using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    [SerializeField] private TMP_InputField nameField;
    [SerializeField] private TMP_Text startButtonText;
    //[SerializeField] private TMP_Dropdown dd_lobbies;

    [SerializeField] private int minNameLength = 1;
    [SerializeField] private int maxNameLength = 12;
    public const string PlayerNameKey = "PlayerName";

    private bool isRefreshing = false;
    private bool gameStarted = false;
    private string joinCode;
    private string joinedRoom;
    private List<string> lstAnimals = new List<string>();

    private void Start()
    {
        populateList();
        string name = PlayerPrefs.GetString(PlayerNameKey, string.Empty);
        if (name == "" || name == string.Empty || name == null)
        {
            int randInt = UnityEngine.Random.Range(0, lstAnimals.Count);
            string randomName = lstAnimals[randInt];
            nameField.text = randomName;
        }
        else {
            nameField.text = name;
        }
        StartCoroutine(RefreshLobbyList(6));
    }

    public void saveName() {
        PlayerPrefs.SetString(PlayerNameKey, nameField.text);
    }

    public async void StartHost() {
        PlayerPrefs.SetString(PlayerNameKey, nameField.text);
        StopAllCoroutines();
        if (!gameStarted)
        {
            await HostSingleton.Instance.GameManager.StartHostAsync();
        }
        else {
            await ClientSingleton.Instance.GameManager.StartClientAsync(joinCode);
            ClientSingleton.Instance.GameManager.joinedHostName = joinedRoom;

        }
    }

    public async void RefreshList()
    {
        if (isRefreshing) return;
        isRefreshing = true;

        QueryLobbiesOptions options = new QueryLobbiesOptions();
        try
        {

            options.Count = 1;
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
        catch (LobbyServiceException ex)
        {
            Debug.Log(ex);

        }
        QueryResponse lobbies = await Lobbies.Instance.QueryLobbiesAsync(options);

        if (lobbies.Results.Count > 0)
        {
            gameStarted = true;
            Lobby l = lobbies.Results[0];
            try
            {
                Lobby joiningLobby = await Lobbies.Instance.JoinLobbyByIdAsync(l.Id);
                Debug.Log("joiningLobby:" + joiningLobby.Id);
                joinCode = joiningLobby.Data["JoinCode"].Value;
                Debug.Log("joincode:" + joinCode);
                joinedRoom = l.Name;
                startButtonText.text = "Join Game";
            }
            catch (LobbyServiceException ex)
            {
                Debug.Log(ex);

            }


        } else {
            gameStarted = false;
            startButtonText.text = "Start Game";
            joinCode = "";
            joinedRoom = "";
        }
        isRefreshing = false;
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

    private void OnApplicationQuit()
    {
        if (HostSingleton.Instance.GameManager != null)
        {
            HostSingleton.Instance.GameManager.Dispose();
        }
    }
    private void populateList()
    {
        lstAnimals.Add("seal");
        lstAnimals.Add("tiger");
        lstAnimals.Add("jaguar");
        lstAnimals.Add("lovebird");
        lstAnimals.Add("parrot");
        lstAnimals.Add("anteater");
        lstAnimals.Add("moose");
        lstAnimals.Add("mouse");
        lstAnimals.Add("zebu");
        lstAnimals.Add("shrew");
        lstAnimals.Add("steer");
        lstAnimals.Add("bull");
        lstAnimals.Add("panther");
        lstAnimals.Add("meerkat");
        lstAnimals.Add("llama");
        lstAnimals.Add("buffalo");
        lstAnimals.Add("blue crab");
        lstAnimals.Add("pig");
        lstAnimals.Add("chipmunk");
        lstAnimals.Add("hamster");
        lstAnimals.Add("octopus");
        lstAnimals.Add("ewe");
        lstAnimals.Add("duckbill platypus");
        lstAnimals.Add("bumble bee");
        lstAnimals.Add("burro");
        lstAnimals.Add("hippopotamus");
        lstAnimals.Add("impala");
        lstAnimals.Add("leopard");
        lstAnimals.Add("jerboa");
        lstAnimals.Add("sheep");
        lstAnimals.Add("waterbuck");
        lstAnimals.Add("mandrill");
        lstAnimals.Add("addax");
        lstAnimals.Add("porcupine");
        lstAnimals.Add("mongoose");
        lstAnimals.Add("rooster");
        lstAnimals.Add("camel");
        lstAnimals.Add("aardvark");
        lstAnimals.Add("gazelle");
        lstAnimals.Add("canary");
        lstAnimals.Add("highland cow");
        lstAnimals.Add("cheetah");
        lstAnimals.Add("wombat");
        lstAnimals.Add("lemur");
        lstAnimals.Add("antelope");
        lstAnimals.Add("argali");
        lstAnimals.Add("hare");
        lstAnimals.Add("musk-ox");
        lstAnimals.Add("sloth");
        lstAnimals.Add("pronghorn");
        lstAnimals.Add("hog");
        lstAnimals.Add("civet");
        lstAnimals.Add("muskrat");
        lstAnimals.Add("bunny");
        lstAnimals.Add("gemsbok");
        lstAnimals.Add("squirrel");
        lstAnimals.Add("armadillo");
        lstAnimals.Add("whale");
        lstAnimals.Add("ibex");
        lstAnimals.Add("gopher");
        lstAnimals.Add("fawn");
        lstAnimals.Add("dingo");
        lstAnimals.Add("musk deer");
        lstAnimals.Add("deer");
        lstAnimals.Add("ocelot");
        lstAnimals.Add("doe");
        lstAnimals.Add("grizzly bear");
        lstAnimals.Add("warthog");
        lstAnimals.Add("lion");
        lstAnimals.Add("kitten");
        lstAnimals.Add("mynah bird");
        lstAnimals.Add("monkey");
        lstAnimals.Add("rat");
        lstAnimals.Add("fox");
        lstAnimals.Add("budgerigar");
        lstAnimals.Add("guinea pig");
        lstAnimals.Add("gnu");
        lstAnimals.Add("snake");
        lstAnimals.Add("zebra");
        lstAnimals.Add("bison");
        lstAnimals.Add("hedgehog");
        lstAnimals.Add("chimpanzee");
        lstAnimals.Add("baboon");
        lstAnimals.Add("lynx");
        lstAnimals.Add("alpaca");
        lstAnimals.Add("ox");
        lstAnimals.Add("thorny devil");
        lstAnimals.Add("marmoset");
        lstAnimals.Add("cow");
        lstAnimals.Add("beaver");
        lstAnimals.Add("gorilla");
        lstAnimals.Add("badger");
        lstAnimals.Add("frog");
        lstAnimals.Add("chameleon");
        lstAnimals.Add("koala");
        lstAnimals.Add("oryx");
        lstAnimals.Add("parakeet");
        lstAnimals.Add("opossum");
        lstAnimals.Add("walrus");
        lstAnimals.Add("dung beetle");
        lstAnimals.Add("polar bear");
        lstAnimals.Add("mule");
        lstAnimals.Add("mustang");
        lstAnimals.Add("kangaroo");
        lstAnimals.Add("panda");
        lstAnimals.Add("reindeer");
        lstAnimals.Add("chinchilla");
        lstAnimals.Add("toad");
        lstAnimals.Add("woodchuck");
        lstAnimals.Add("finch");
        lstAnimals.Add("chicken");
        lstAnimals.Add("colt");
        lstAnimals.Add("mink");
        lstAnimals.Add("salamander");
        lstAnimals.Add("lamb");
        lstAnimals.Add("coati");
        lstAnimals.Add("wildcat");
        lstAnimals.Add("okapi");
        lstAnimals.Add("rabbit");
        lstAnimals.Add("puma");
        lstAnimals.Add("capybara");
        lstAnimals.Add("vicuna");
        lstAnimals.Add("otter");
        lstAnimals.Add("horse");
        lstAnimals.Add("puppy");
        lstAnimals.Add("dromedary");
        lstAnimals.Add("coyote");
        lstAnimals.Add("silver fox");
        lstAnimals.Add("prairie dog");
        lstAnimals.Add("ram");
        lstAnimals.Add("rhinoceros");
        lstAnimals.Add("elk");
        lstAnimals.Add("turtle");
        lstAnimals.Add("hartebeest");
        lstAnimals.Add("giraffe");
        lstAnimals.Add("chamois");
        lstAnimals.Add("donkey");
        lstAnimals.Add("peccary");
        lstAnimals.Add("ape");
        lstAnimals.Add("lizard");
        lstAnimals.Add("guanaco");
        lstAnimals.Add("weasel");
        lstAnimals.Add("orangutan");
        lstAnimals.Add("cougar");
        lstAnimals.Add("fish");
        lstAnimals.Add("mountain goat");
        lstAnimals.Add("porpoise");
        lstAnimals.Add("ground hog");
        lstAnimals.Add("starfish");
        lstAnimals.Add("bighorn");
        lstAnimals.Add("newt");
        lstAnimals.Add("elephant");
        lstAnimals.Add("mare");
        lstAnimals.Add("dormouse");
        lstAnimals.Add("alligator");
        lstAnimals.Add("bear");
        lstAnimals.Add("eland");
        lstAnimals.Add("crocodile");
        lstAnimals.Add("stallion");
        lstAnimals.Add("mole");
        lstAnimals.Add("bald eagle");
        lstAnimals.Add("skunk");
        lstAnimals.Add("ermine");
        lstAnimals.Add("boar");
        lstAnimals.Add("marten");
        lstAnimals.Add("hyena");
        lstAnimals.Add("tapir");
        lstAnimals.Add("snowy owl");
        lstAnimals.Add("pony");
        lstAnimals.Add("cat");
        lstAnimals.Add("yak");
        lstAnimals.Add("wolverine");
        lstAnimals.Add("aoudad");
        lstAnimals.Add("eagle owl");
        lstAnimals.Add("raccoon");
        lstAnimals.Add("basilisk");
        lstAnimals.Add("iguana");
        lstAnimals.Add("dog");
        lstAnimals.Add("jackal");
        lstAnimals.Add("wolf");
        lstAnimals.Add("crow");
        lstAnimals.Add("ferret");
        lstAnimals.Add("springbok");
        lstAnimals.Add("goat");
        lstAnimals.Add("dugong");
        lstAnimals.Add("gila monster");
        lstAnimals.Add("bat");
        lstAnimals.Add("quagga");

    }

}
