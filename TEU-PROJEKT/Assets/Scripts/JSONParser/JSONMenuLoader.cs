using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class Shared
{
    public string restart { get; set; }
    public string changeRegion { get; set; }
    public string quit { get; set; }
}
public class Pause
{
    public string resume { get; set; }
}

public class End
{
    public string score { get; set; }
    public string highscore { get; set; }
}

public class InGame
{
    public string correct { get; set; }
    public string select { get; set; }
}

public class Menu
{
    public string language { get; set; }
    public Shared shared { get; set; }
    public Pause pause { get; set; }
    public End end { get; set; }
    public InGame inGame { get; set; }
}

public class Root
{
    public List<Menu> menus { get; set; }
}

public class JSONMenuLoader : MonoBehaviour
{
    public List<Menu> Menus { get; private set; }
    public event EventHandler<string> OnLanguageChange;
    public string currentLanguage = "hr";
    private bool dataLoaded = false;
    void Start()
    {
        LoadLanguages();
    }

    void LoadLanguages()
    {
        string jsonPath = "/Data/PauseMenu.json";
        string fullPath = Application.dataPath + jsonPath;
        //Languages = new List<Language>();
        //Debug.Log($"PATH: {fullPath}");
        Menus = new List<Menu>();
        
        if (File.Exists(fullPath))
        {
            string jsonString = File.ReadAllText(fullPath);
            Root root = JsonConvert.DeserializeObject<Root>(jsonString);

            //Debug.Log(root.menus);
            if (root != null && root.menus != null)
            {
                foreach (Menu menu in root.menus)
                {
                    Menus.Add(menu);
                }
                dataLoaded = true;
                Debug.Log("LANGUAGES LOADED");
            }
            else
            {
                Debug.LogError("Failed to parse JSON or no languages detected");
            }

            //after loading data, languages have to match
            //THIS DOES NOT WORK!!!!
            OnLanguageChange?.Invoke(this, currentLanguage);
        }
        else
        {
            Debug.LogError($"JSON file not found at path: {fullPath}");
        }
    }
    public void ChangeLanguage(string selectedLanguage)
    {
        currentLanguage = selectedLanguage;
        OnLanguageChange?.Invoke(this, selectedLanguage);
    }

    public Menu GetUpdatedMenu()
    {
        return Menus.Find(menu => menu.language == currentLanguage);
    }
    public bool DataLoaded()
    {
        return dataLoaded;
    }
}
