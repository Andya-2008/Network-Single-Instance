using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class popAudioDropDownScript : MonoBehaviour
{
    [SerializeField] GameObject gameController;

    // Start is called before the first frame update
    void Start()
    {
        TMP_Dropdown dd_audio = this.GetComponent<TMP_Dropdown>();
        dd_audio.ClearOptions();
        // Finding all the music files
        AudioClip[] clips = Resources.LoadAll<AudioClip>("sounds/");
        Debug.Log(clips.Length);
        List<string> lstAudio = new List<string>();

        foreach (AudioClip c in clips) {
            lstAudio.Add(c.name);
        }

        /*
        FileInfo[] options = getAudioFiles();
        // add Options to the dropdown from list
        foreach (FileInfo data in options)
        {
            if (!data.Name.Contains(".meta"))
            {
                lstAudio.Add(Path.GetFileNameWithoutExtension(data.Name));
            }
        }
        */
        dd_audio.AddOptions(lstAudio);

        gameController.GetComponent<GameControllerScript>().registerSoundServerRpc(dd_audio.options[dd_audio.value].text);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private FileInfo[] getAudioFiles()
    {
        string path = Application.dataPath + "/Resources/sounds";

        Debug.Log(path);
        // How to check if exists?

        DirectoryInfo audioFolder = new DirectoryInfo(@path);
        FileInfo[] audioFiles = audioFolder.GetFiles();
        return audioFiles;

    }

    public void registerSound() {
        TMP_Dropdown dd_audio = this.GetComponent<TMP_Dropdown>();
        gameController.GetComponent<GameControllerScript>().registerSoundServerRpc(dd_audio.options[dd_audio.value].text);

    }


}
