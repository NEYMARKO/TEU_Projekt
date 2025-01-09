using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class EndGameMenuUI : MenuBaseUI
{
    [SerializeField] private TextMeshProUGUI scoreTextObj;
    [SerializeField] private TextMeshProUGUI highScoreTextObj;

    void Start()
    {
        SetupButtonListener();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    public override void HandleContentChange(object sender, Menu menu)
    {
        base.HandleContentChange(sender, menu);
        scoreTextObj.text = $"{menu.end.score.ToUpper()}: {base.gameLoop.GetCorrectAnswersCount()}";
        highScoreTextObj.text = $"{menu.end.highscore.ToUpper()}: {gameLoop.GetHighScore()}";
        //highScoreTextObj.text = $"{menu.end.highscore.ToUpper()}: ";
    }

    //prefabs can't have assigned functions through user interface, it has to be assigned through script
    private void SetupButtonListener()
    {
        RestartButton.onClick.AddListener((UnityEngine.Events.UnityAction)this.RestartGame);
    }
    public override void RestartGame()
    {
        //because button group is child of actual menu
        base.gameLoop.RestartGame(gameObject);
    }
}
