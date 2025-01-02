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
    [Header("Buttons")]
    [SerializeField] protected Button RestartButton;
    [Header("Buttons Text")]
    [SerializeField] protected TextMeshProUGUI restartButtonTextObj;
    [SerializeField] protected TextMeshProUGUI changeRegionButtonTextObj;
    [SerializeField] protected TextMeshProUGUI quitButtonTextObj;

    private void Awake()
    {
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
        SetupButtonListener();
        SetupButtons();
    }

    private void OnEnable()
    {
        // Subscribe to the event
        menuDropdown.OnLanguageChange += HandleLanguageChange;
    }

    private void OnDisable()
    {
        // Unsubscribe from the event to avoid memory leaks
        menuDropdown.OnLanguageChange -= HandleLanguageChange;
    }

    private void HandleLanguageChange(object sender, Menu menu)
    {
        //Debug.Log($"Language changed to: {menu.language}");
        restartButtonTextObj.text = menu.shared.restart.ToUpper();
        changeRegionButtonTextObj.text = menu.shared.changeRegion.ToUpper();
        quitButtonTextObj.text = menu.shared.quit.ToUpper();
    }
    private void SetupButtons()
    {
        restartButtonTextObj.text = $"RESET";
        changeRegionButtonTextObj.text = $"CHANGE LEVEL";
        quitButtonTextObj.text = $"QUIT";
    }

    //prefabs can't have assigned functions through user interface, it has to be assigned through script
    private void SetupButtonListener()
    {
        RestartButton.onClick.AddListener((UnityEngine.Events.UnityAction)this.RestartGame);
    }
    public void RestartGame()
    {
        //because button group is child of actual menu
        gameLoop.RestartGame(transform.parent.gameObject);
    }

    public GameLoop GetGameLoop()
    { 
        return gameLoop; 
    }
}
