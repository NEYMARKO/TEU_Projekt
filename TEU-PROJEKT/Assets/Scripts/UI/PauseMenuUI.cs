using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuUI : MenuBaseUI
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private TextMeshProUGUI resumeButtonTextObj;
    // Start is called before the first frame update
    private void Awake()
    {
        gameLoop = GetComponent<GameLoop>();
    }
    void Start()
    {
        SetupButtonListener();
        SetupButtons();
    }

    // Update is called once per frame
    private void SetupButtons()
    {
        resumeButtonTextObj.text = $"RESUME";
    }

    //prefabs can't have assigned functions through user interface, it has to be assigned through script
    private void SetupButtonListener()
    {
        resumeButton.onClick.AddListener((UnityEngine.Events.UnityAction)this.ResumeGame);
    }
    public void ResumeGame()
    {
        //because button group is child of actual menu
        gameLoop.ResumeGame(transform.parent.gameObject);
    }
}
