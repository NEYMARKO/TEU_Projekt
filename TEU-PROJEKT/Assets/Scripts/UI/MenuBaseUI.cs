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
    [SerializeField] protected Button QuitButton;
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

    }
    void Start()
    {
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
            GetAndUpdateCurrentMenu();
            notLoaded = false;
        }
    }
    protected virtual void OnEnable()
    {
        notLoaded = true;
        menuDropdown.OnMenuContentChange += HandleContentChange;
        if (!menuDropdown.IsMenuContentLoaderInitialized()) return;
        //Debug.Log($"ACTIVE LANGUAGE: {activeLanguage}, DROPDOWN LANGUAGE: {menuDropdown.GetActiveLanguage()}");
        if (activeLanguage == null || activeLanguage != menuDropdown.GetActiveLanguage())
        {
            GetAndUpdateCurrentMenu();
            notLoaded = false;
        }
    }

    private void OnDisable()
    {
        menuDropdown.OnMenuContentChange -= HandleContentChange;
    }

    public virtual void HandleContentChange(object sender, Menu menu)
    {
        activeLanguage = menu.language;
        restartButtonTextObj.text = menu.shared.restart.ToUpper();
        changeRegionButtonTextObj.text = menu.shared.changeRegion.ToUpper();
        quitButtonTextObj.text = menu.shared.quit.ToUpper();
    }

    //prefabs can't have assigned functions through user interface, it has to be assigned through script
    private void SetupButtonListener()
    {
        RestartButton.onClick.AddListener((UnityEngine.Events.UnityAction)this.RestartGame);
        QuitButton.onClick.AddListener((UnityEngine.Events.UnityAction)this.QuitGame);
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

    public void QuitGame()
    {
        Debug.Log("quit");
        Application.Quit();
    }
    public GameLoop GetGameLoop()
    { 
        return gameLoop; 
    }
}
