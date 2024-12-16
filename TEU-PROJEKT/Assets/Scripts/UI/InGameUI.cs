using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InGameUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI wantedCityText;
    [SerializeField] GameLoop gameLoop;
    //starting text is going to be chosen depending on what language was chosen in settings
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = $"Correctly answered: {gameLoop.GetCorrectAnswersCount()} / {gameLoop.GetCitiesCount()}";
        wantedCityText.text = $"SELECT: {gameLoop.GetWantedCity()}";

    }
}
