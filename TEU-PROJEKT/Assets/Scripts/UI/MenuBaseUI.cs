using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MenuBaseUI : MonoBehaviour
{
    [SerializeField] protected PauseMenuDropdown menuDropdown;
    /*[SerializeField]*/ protected GameLoop gameLoop;
    [SerializeField] protected Button RestartButton;
    [SerializeField] protected TextMeshProUGUI restartButtonTextObj;
    [SerializeField] protected TextMeshProUGUI changeRegionButtonTextObj;
    [SerializeField] protected TextMeshProUGUI quitButtonTextObj;

    private string activeLanguage;
    private bool notLoaded = true;
    private void Awake()
    {
        activeLanguage = null;
        GameObject playerObject = GameObject.Find("Player");
        if (playerObject == null)
        {
            Debug.LogError("Player not found in the scene!");
        }
        else 
        {
            gameLoop = playerObject.GetComponent<GameLoop>();
        }
        //disable script so OnEnabled wouldn't get called before
        //menuContentLoader could be initialized in PauseMenuDropdown.cs
        //this.enabled = false;
    }
    void Start()
    {
        //this.enabled = true;
        if (notLoaded)
        {
            GetAndUpdateCurrentMenu();
            notLoaded = false;
        }
        SetupButtonListener();
    }

    private void Update()
    {
        if (notLoaded && menuDropdown.IsMenuContentLoaderInitialized())
        {
            //Debug.Log("IN UPDATE");
            GetAndUpdateCurrentMenu();
            notLoaded = false;
        }
    }
    protected virtual void OnEnable()
    {
        //Debug.Log("SCRIPT ENABLED");
        notLoaded = true;
        menuDropdown.OnMenuContentChange += HandleContentChange;
        if (!menuDropdown.IsMenuContentLoaderInitialized()) return;
        //Debug.Log($"ACTIVE LANGUAGE: {activeLanguage}, DROPDOWN LANGUAGE: {menuDropdown.GetActiveLanguage()}");
        if (activeLanguage == null || activeLanguage != menuDropdown.GetActiveLanguage())
        {
            GetAndUpdateCurrentMenu();
            notLoaded = false;
        }
        // Subscribe to the event
    }

    private void OnDisable()
    {
        // Unsubscribe from the event to avoid memory leaks
        menuDropdown.OnMenuContentChange -= HandleContentChange;
    }

    public virtual void HandleContentChange(object sender, Menu menu)
    {
        //Debug.Log("CHANGING STUFF");
        activeLanguage = menu.language;
        //Debug.Log($"Language changed to: {menu.language}");
        restartButtonTextObj.text = menu.shared.restart.ToUpper();
        changeRegionButtonTextObj.text = menu.shared.changeRegion.ToUpper();
        quitButtonTextObj.text = menu.shared.quit.ToUpper();
    }

    //prefabs can't have assigned functions through user interface, it has to be assigned through script
    private void SetupButtonListener()
    {
        RestartButton.onClick.AddListener((UnityEngine.Events.UnityAction)this.RestartGame);
    }

    private void GetAndUpdateCurrentMenu()
    {
        Menu menu = menuDropdown.GetActiveMenu();
        //Debug.Log($"UPDATED MENU lang: {menu.language}");
        if (menu != null)
        {
            HandleContentChange(this, menu);
        }
    }
    public virtual void RestartGame()
    {
        //because button group is child of actual menu
        gameLoop.RestartGame(transform.parent.gameObject);
    }

    public GameLoop GetGameLoop()
    { 
        return gameLoop; 
    }
}
