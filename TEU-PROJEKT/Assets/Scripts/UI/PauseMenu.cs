using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] PauseMenuDropdown menuDropdown;
    [SerializeField] GameLoop gameLoop;
    [Header("Buttons")]
    [SerializeField] TextMeshProUGUI resumeButtonTextObj;
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
        menuDropdown.OnMenuContentChange += HandleLanguageChange;
    }

    private void OnDisable()
    {
        // Unsubscribe from the event to avoid memory leaks
        menuDropdown.OnMenuContentChange -= HandleLanguageChange;
    }

    private void HandleLanguageChange(object sender, Menu menu)
    {
        Debug.Log($"Language changed to: {menu.language}");
        resumeButtonTextObj.text = menu.pause.resume.ToUpper();
        resetButtonTextObj.text = menu.shared.restart.ToUpper();
        changeLevelButtonTextObj.text = menu.shared.changeRegion.ToUpper();
        quitButtonTextObj.text = menu.shared.quit.ToUpper();
    }
    private void SetupButtons()
    {
        resumeButtonTextObj.text = $"RESUME";
        resetButtonTextObj.text = $"RESET";
        changeLevelButtonTextObj.text = $"CHANGE LEVEL";
        quitButtonTextObj.text = $"QUIT";
    }

    public void ResumeGame()
    {
        gameLoop.ResumeGame(gameObject);
    }
    public void RestartGame()
    {
        gameLoop.RestartGame(gameObject);
    }
}
