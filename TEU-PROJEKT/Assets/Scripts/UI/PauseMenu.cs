using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] PauseMenuDropdown menuDropdown;
    [SerializeField] GameLoop gameLoop;
    [Header("Buttons")]
    [SerializeField] TextMeshProUGUI optionsButtonTextObj;
    [SerializeField] TextMeshProUGUI resetButtonTextObj;
    [SerializeField] TextMeshProUGUI changeLevelButtonTextObj;
    [SerializeField] TextMeshProUGUI quitButtonTextObj;
    private int score;
    void Start()
    {
        SetupButtons();
        //UpdateScore();
    }

    // Update is called once per frame
    void Update()
    {
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

    private void HandleLanguageChange(object sender, Language language)
    {
        Debug.Log($"Language changed to: {language.language}");
        resetButtonTextObj.text = language.restart;
        changeLevelButtonTextObj.text = language.changeRegion;
        quitButtonTextObj.text = language.quit;
    }
    private void SetupButtons()
    {
        optionsButtonTextObj.text = $"OPTIONS";
        resetButtonTextObj.text = $"RESET";
        changeLevelButtonTextObj.text = $"CHANGE LEVEL";
        quitButtonTextObj.text = $"QUIT";
    }

    public void RestartGame()
    {
        gameLoop.RestartGame(gameObject);
    }
}
