using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuUI : MenuBaseUI
{
    //[SerializeField] private MenuBaseUI baseMenu;
    [SerializeField] private Button resumeButton;
    [SerializeField] private TextMeshProUGUI resumeButtonTextObj;

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
        resumeButtonTextObj.text = menu.pause.resume.ToUpper();
    }
    private void SetupButtonListener()
    {
        resumeButton.onClick.AddListener((UnityEngine.Events.UnityAction)this.ResumeGame);
    }
    public void ResumeGame()
    {
        //because button group is child of actual menu
        base.gameLoop.ResumeGame(gameObject);
    }
}
