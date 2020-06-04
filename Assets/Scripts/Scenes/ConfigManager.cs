using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigManager : MonoBehaviour
{
    public static ConfigFile config;
    public static ConfigManager instance;

    public static void SaveIntoJson()
    {
        string config = JsonUtility.ToJson(ConfigManager.config, true);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/config.json", config);
        
    }

    public static void ReadFromJson()
    {
        string config = System.IO.File.ReadAllText(Application.persistentDataPath + "/config.json");

        ConfigManager.config = JsonUtility.FromJson<ConfigFile>(config);
    }

    public void Awake()
    {

        if(instance == null)
        {
            instance = GetComponent<ConfigManager>();
        }
        else
        {
            Destroy(GetComponent<ConfigManager>());
        }

        

        

        
    }

    private void OnEnable()
    {
        ConfigManager.config = new ConfigFile();

        if (!System.IO.File.Exists(Application.persistentDataPath + "/config.json"))
        {
            SaveIntoJson();
        }
        ReadFromJson();
    }
}

[System.Serializable]
public class ConfigFile
{
    public float MouseSensitivity = 69f;
}
