using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Language
{
    public string language { get; set; }
    public string resume { get; set; }
    public string restart { get; set; }
    public string changeRegion { get; set; }
    public string cancel { get; set; }
    public string quit { get; set; }
}
public class Root
{
    public List<Language> languages { get; set; }
}
public class JSONMenuLoader : MonoBehaviour
{
    public List<Language> Languages { get; private set; }
    private string currentLanguage = "hr";
    // Start is called before the first frame update
    void Start()
    {
        LoadLanguages();
        UpdateUI(currentLanguage);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void LoadLanguages()
    {
        string jsonPath = "/Data/PauseMenu.json";
        string fullPath = Application.dataPath + jsonPath;
        Languages = new List<Language>();
        
        if (File.Exists(fullPath))
        {
            string jsonString = File.ReadAllText(fullPath);
            Root root = JsonConvert.DeserializeObject<Root>(jsonString);

            if (root != null && root.languages != null)
            {
                foreach (Language language in root.languages)
                {
                    Languages.Add(language);
                }
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
        UpdateUI(currentLanguage);
    }

    void UpdateUI(string languageCode)
    {
        Language selectedLanguage = Languages.Find(lang => lang.language == languageCode);
        if (selectedLanguage != null)
        {
            Debug.Log($"Resume: {selectedLanguage.resume}");
            Debug.Log($"Quit: {selectedLanguage.quit}");
            // Postavi tekst na UI gumbe itd.
        }
        else
        {
            Debug.LogError($"Language '{languageCode}' not found!");
        }
    }
}
