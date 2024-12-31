using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuDropdown : MonoBehaviour
{
    public TMPro.TMP_Dropdown languageDropdown;
    public JSONMenuLoader pauseMenuLoader;

    private bool initializedMenu = false;
    void Start()
    {
        PopulateDropdown();
        //languageDropdown.captionText.text = pauseMenuLoader.currentLanguage;
        languageDropdown.onValueChanged.AddListener(OnLanguageSelected);
    }
    private void Update()
    {
        if (!initializedMenu)
        {
            PopulateDropdown();
        }
    }
    void PopulateDropdown()
    {
        //languages haven't been parsed yet
        if(!pauseMenuLoader.DataLoaded()) return;

        languageDropdown.ClearOptions();
        List<string> options = new List<string>();
        foreach (var language in pauseMenuLoader.Languages)
        {
            options.Add(language.language);
        }
        languageDropdown.AddOptions(options);
        initializedMenu = true;
    }

    void OnLanguageSelected(int index)
    {
        string selectedLanguage = pauseMenuLoader.Languages[index].language;
        pauseMenuLoader.ChangeLanguage(selectedLanguage);
    }
}
