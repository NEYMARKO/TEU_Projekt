using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InGameUI : MonoBehaviour
{
    private GameObject languageController;
    private JSONMenuLoader menuContentLoader;
    [SerializeField] PauseMenuDropdown pauseMenuDropdown;
    [SerializeField] PauseMenuDropdown endGameMenuDropdown;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI wantedCityText;
    [SerializeField] TextMeshProUGUI elapsedTimeText;
    [SerializeField] GameLoop gameLoop;
 
    private string scoreString;
    private string wantedCityString;
    private string elapsedTimeString;
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
        }
    }

    private void Start()
    {
        Menu menu = menuContentLoader.GetUpdatedMenu();
        scoreString = menu.inGame.correct.ToUpper();
        wantedCityString = menu.inGame.select.ToUpper();
        elapsedTimeString = menu.inGame.elapsedTime.ToUpper();
    }
    private void OnEnable()
    {
        //since there are 2 dropdowns which can change language, InGame text needs to subscribe to both
        //and not just one: if it is subscribed only to pause menu, if changes are made in end game menu
        //they wont be visible on InGame text (opposite is the same)
        pauseMenuDropdown.OnMenuContentChange += HandleContentChange;
        endGameMenuDropdown.OnMenuContentChange += HandleContentChange;
    }

    private void OnDisable()
    {
        pauseMenuDropdown.OnMenuContentChange -= HandleContentChange;
        endGameMenuDropdown.OnMenuContentChange -= HandleContentChange;
    }
    public void HandleContentChange(object sender, Menu menu)
    {
        scoreString = menu.inGame.correct.ToUpper();
        wantedCityString = menu.inGame.select.ToUpper();
        elapsedTimeString = menu.inGame.elapsedTime.ToUpper();
    }
    void Update()
    {
        scoreText.text = $"{scoreString}: {gameLoop.GetCorrectAnswersCount()} / {gameLoop.GetCitiesCount()}";
        wantedCityText.text = $"{wantedCityString}: {gameLoop.GetWantedCity()}";
        elapsedTimeText.text = $"{elapsedTimeString}: {gameLoop.GetElapsedTime()}";
    }
}
