using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Menu : MonoBehaviour
{
    //this has to be public for children to be able to access it
    public GameLoop gameLoop;
    [Header("Buttons")]
    [SerializeField] TextMeshProUGUI resetButtonTextObj;
    [SerializeField] TextMeshProUGUI changeLevelButtonTextObj;
    [SerializeField] TextMeshProUGUI quitButtonTextObj;
    void Start()
    {
        SetupButtons();
    }

    // Update is called once per frame
    void Update()
    {
    }

    
    private void SetupButtons()
    {
        resetButtonTextObj.text = $"RESET";
        changeLevelButtonTextObj.text = $"CHANGE LEVEL";
        quitButtonTextObj.text = $"QUIT";
    }

    private void QuitGame()
    {
        Debug.Log("QUIT CLICKED");
    }
    //public void RestartGame()
    //{
    //    gameLoop.RestartGame();
    //}
}
