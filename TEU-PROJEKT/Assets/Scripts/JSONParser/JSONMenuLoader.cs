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
    //public List<Language> Languages { get; private set; }
    public List<Menu> Menus { get; private set; }
    public event EventHandler<string> OnLanguageChange;
    public string currentLanguage = "hr";
    private bool dataLoaded = false;
    // Start is called before the first frame update
    void Start()
    {
        LoadLanguages();
        //UpdateUI(currentLanguage);
    }

    // Update is called once per frame
    void Update()
    {
        
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
        //return Menus.Find(menu => menu.language == selectedLanguage);
    }

    public Menu GetUpdatedMenu()
    {
        return Menus.Find(menu => menu.language == currentLanguage);
    }

    //void UpdateUI(string languageCode)
    //{
    //    Menu selectedLanguage = Menus.Find(lang => lang.language == languageCode);
    //    if (selectedLanguage != null)
    //    {
    //        Debug.Log($"Resume: {selectedLanguage.pause.resume}");
    //        Debug.Log($"Quit: {selectedLanguage.shared.quit}");
    //        // Postavi tekst na UI gumbe itd.
    //    }
    //    else
    //    {
    //        Debug.LogError($"Language '{languageCode}' not found!");
    //    }
    //}

    public bool DataLoaded()
    {
        return dataLoaded;
    }
}
