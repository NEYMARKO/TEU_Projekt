using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
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
