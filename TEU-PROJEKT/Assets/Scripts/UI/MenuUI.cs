using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuUI : MonoBehaviour
{
    [SerializeField] GameLoop gameLoop;
    [SerializeField] TextMeshProUGUI scoreText;
    [Header("Buttons")]
    [SerializeField] TextMeshProUGUI resetButtonTextObj;
    [SerializeField] TextMeshProUGUI changeLevelButtonTextObj;
    [SerializeField] TextMeshProUGUI quitButtonTextObj;
    private int score;
    void Start()
    {
        SetupButtons();
        UpdateScore();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateScore()
    {
        score = gameLoop.GetCorrectAnswersCount();
        scoreText.text = $"SCORE: {score}";
    }
    private void SetupButtons()
    {
        resetButtonTextObj.text = $"RESET";
        changeLevelButtonTextObj.text = $"CHANGE LEVEL";
        quitButtonTextObj.text = $"QUIT";
    }
}
