using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuDropdown : MonoBehaviour
{
    public TMPro.TMP_Dropdown languageDropdown;
    private JSONMenuLoader menuContentLoader;
    private GameObject languageController;
    public event EventHandler<Menu> OnMenuContentChange;
    private bool initializedMenu = false;
    private Menu currentMenu;

    private void Awake()
    {
        languageController = GameObject.Find("LanguageController");
        if (languageController == null)
        {
            Debug.LogError("LanguageController not found in the scene!");
        }
        else
        {
            if (!menuContentLoader) menuContentLoader = languageController.GetComponent<JSONMenuLoader>();
            //Debug.Log($"MENU CONTENT LOADER IN AWAKE NULL:{(menuContentLoader == null ? "TRUE" : "FALSE")}");
        }
    }
    void Start()
    {
        PopulateDropdown();
        if (menuContentLoader.Menus[languageDropdown.value].language
            != menuContentLoader.currentLanguage)
        {
            languageDropdown.value = menuContentLoader.Menus.FindIndex(menu =>
            menu.language == menuContentLoader.currentLanguage);
            languageDropdown.RefreshShownValue();
        }
        languageDropdown.onValueChanged.AddListener(OnLanguageSelected);
    }
    private void Update()
    {
        if (!initializedMenu)
        {
            PopulateDropdown();
        }
    }

    private void OnEnable()
    {
        if (!menuContentLoader) return;
        if (menuContentLoader.Menus[languageDropdown.value].language 
            != menuContentLoader.currentLanguage)
        {
            languageDropdown.value = menuContentLoader.Menus.FindIndex(menu =>
            menu.language == menuContentLoader.currentLanguage);
            languageDropdown.RefreshShownValue();
        }
        menuContentLoader.OnLanguageChange += HandleLanguageChange;
    }

    private void OnDisable()
    {
        // Unsubscribe from the event to avoid memory leaks
        menuContentLoader.OnLanguageChange -= HandleLanguageChange;
    }
    void PopulateDropdown()
    {
        //languages haven't been parsed yet
        if(!menuContentLoader.DataLoaded()) return;

        languageDropdown.ClearOptions();
        List<string> options = new List<string>();
        foreach (var menu in menuContentLoader.Menus)
        {
            options.Add(menu.language);
        }
        languageDropdown.AddOptions(options);
        initializedMenu = true;
    }

    private void HandleLanguageChange(object sender, string newLanguage)
    {
        currentMenu = menuContentLoader.GetUpdatedMenu();
        OnMenuContentChange?.Invoke(this, currentMenu);
    }
    void OnLanguageSelected(int index)
    {
        string selectedLanguage = menuContentLoader.Menus[index].language;
        menuContentLoader.ChangeLanguage(selectedLanguage);
    }

    public bool IsMenuContentLoaderInitialized()
    {
        return menuContentLoader != null;
    }
    public Menu GetActiveMenu()
    {
        return menuContentLoader.GetUpdatedMenu();
    }

    public string GetActiveLanguage()
    {
        return menuContentLoader.currentLanguage;
    }
}
