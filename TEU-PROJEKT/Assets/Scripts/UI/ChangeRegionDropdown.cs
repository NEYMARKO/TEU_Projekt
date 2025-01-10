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
    public event EventHandler<int> OnRegionChange;
    [SerializeField] GameLoop gameLoop;
 
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
        }
    }
    void Start()
    {
        regions = new List<string>();
        PopulateDropdown();
     
        if (changeRegionDropdown.value != dbManager.GetActiveRegionID())
        {
            changeRegionDropdown.value = dbManager.GetActiveRegionID();
        }
        changeRegionDropdown.onValueChanged.AddListener(OnRegionSelected);
    }

    private void OnEnable()
    {
        if (regions == null || regions.Count == 0) return;
        if (changeRegionDropdown.value != dbManager.GetActiveRegionID())
        {
            changeRegionDropdown.value = dbManager.GetActiveRegionID();
        }
    }
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
        //initializedMenu = true;
    }

    void OnRegionSelected(int index)
    {
        string selectedRegion = regions[index];
        if (index != dbManager.GetActiveRegionID())
        {
            OnRegionChange?.Invoke(this, index);
            dbManager.SetActiveRegionID(index);
            gameLoop.ReloadLevel(index, transform.parent.gameObject);
        }
    }
}
