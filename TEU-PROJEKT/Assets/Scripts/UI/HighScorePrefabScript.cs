using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HighScorePrefabScript : MonoBehaviour
{
    public TextMeshProUGUI positionTextObj;
    public TextMeshProUGUI studentIDTextObj;
    public TextMeshProUGUI scoreTextObj;
    public TextMeshProUGUI timeTextObj;

    public void SetScoreProperties(int position, string studentID, int score, float time)
    {
        positionTextObj.text = "#" + position.ToString();
        studentIDTextObj.text = studentID;
        scoreTextObj.text= score.ToString();
        timeTextObj.text= time.ToString();
    }
}
