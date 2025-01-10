using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class ChangeRegionDropdown : MonoBehaviour
{
    public TMPro.TMP_Dropdown changeRegionDropdown;
    private DBManager dbManager;
    private GameObject dbManagerObject;
    public event EventHandler<Menu> OnRegionChange;
    private bool initializedMenu = false;
    private Menu currentMenu;

    private List<string> regions;
    private void Awake()
    {
        dbManagerObject = GameObject.Find("DBManager");
        if (dbManagerObject == null)
        {
            Debug.LogError("LanguageController not found in the scene!");
        }
        else
        {
            dbManager = dbManagerObject.GetComponent<DBManager>();
            //Debug.Log($"MENU CONTENT LOADER IN AWAKE NULL:{(menuContentLoader == null ? "TRUE" : "FALSE")}");
        }
    }
    void Start()
    {
        regions = new List<string>();
        PopulateDropdown();
        //if (menuContentLoader.Menus[languageDropdown.value].language
        //    != menuContentLoader.currentLanguage)
        //{
        //    languageDropdown.value = menuContentLoader.Menus.FindIndex(menu =>
        //    menu.language == menuContentLoader.currentLanguage);
        //    languageDropdown.RefreshShownValue();
        //}
        changeRegionDropdown.onValueChanged.AddListener(OnRegionSelected);
    }
    private void Update()
    {
        if (!initializedMenu)
        {
            PopulateDropdown();
        }
    }

    //private void OnEnable()
    //{
    //    //if (!menuContentLoader) return;
    //    if (menuContentLoader.Menus[languageDropdown.value].language
    //        != menuContentLoader.currentLanguage)
    //    {
    //        languageDropdown.value = menuContentLoader.Menus.FindIndex(menu =>
    //        menu.language == menuContentLoader.currentLanguage);
    //        languageDropdown.RefreshShownValue();
    //    }
    //    menuContentLoader.OnLanguageChange += HandleLanguageChange;
    //}

    //private void OnDisable()
    //{
    //    // Unsubscribe from the event to avoid memory leaks
    //    menuContentLoader.OnLanguageChange -= HandleLanguageChange;
    //}

    void PopulateDropdown()
    {
        //languages haven't been parsed yet
        //if (!menuContentLoader.DataLoaded()) return;

        changeRegionDropdown.ClearOptions();
        List<string> options = new List<string>();
        regions = dbManager.GetRegions();
        //List<RegionInfo> regions = new List<RegionInfo>();
        foreach (var regionName in regions)
        {
            options.Add(regionName);
        }
        changeRegionDropdown.AddOptions(options);
        initializedMenu = true;
    }

    //private void HandleLanguageChange(object sender, string newLanguage)
    //{
    //    currentMenu = menuContentLoader.GetUpdatedMenu();
    //    OnMenuContentChange?.Invoke(this, currentMenu);
    //}
    void OnRegionSelected(int index)
    {
        string selectedRegion = regions[index];
        Debug.Log($"SELECTED REGION: {selectedRegion}");
        //menuContentLoader.ChangeLanguage(selectedLanguage);
        //currentMenu = menuContentLoader.GetUpdatedMenu();
        //currentMenu = menuContentLoader.ChangeLanguage(selectedLanguage);
        //OnMenuContentChange?.Invoke(this, currentMenu);
    }

    //public bool IsMenuContentLoaderInitialized()
    //{
    //    return menuContentLoader != null;
    //}
    //public Menu GetActiveMenu()
    //{
    //    //Debug.Log($"CONTENT LOADER IS {(menuContentLoader == null ? "" : "NOT")} NULL");
    //    return menuContentLoader.GetUpdatedMenu();
    //}

    //public string GetActiveLanguage()
    //{
    //    return menuContentLoader.currentLanguage;
    //}
}
