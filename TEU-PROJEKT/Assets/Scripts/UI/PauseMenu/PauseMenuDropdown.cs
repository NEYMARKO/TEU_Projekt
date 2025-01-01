using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuDropdown : MonoBehaviour
{
    public TMPro.TMP_Dropdown languageDropdown;
    public JSONMenuLoader pauseMenuLoader;
    public event EventHandler<Menu> OnLanguageChange;
    private bool initializedMenu = false;
    private Menu currentMenu;
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
        foreach (var menu in pauseMenuLoader.Menus)
        {
            options.Add(menu.language);
        }
        languageDropdown.AddOptions(options);
        initializedMenu = true;
    }

    void OnLanguageSelected(int index)
    {
        string selectedLanguage = pauseMenuLoader.Menus[index].language;
        currentMenu = pauseMenuLoader.ChangeLanguage(selectedLanguage);
        OnLanguageChange?.Invoke(this, currentMenu);
    }  
}
