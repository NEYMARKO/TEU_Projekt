using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class EndGameMenu : Menu
{
    [SerializeField] TextMeshProUGUI scoreText;
    private int score;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateScore();
    }

    private void UpdateScore()
    {
        score = gameLoop.GetCorrectAnswersCount();
        scoreText.text = $"SCORE: {score}";
    }
}
