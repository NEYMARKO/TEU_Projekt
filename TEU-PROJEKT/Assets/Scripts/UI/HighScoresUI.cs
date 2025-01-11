using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score
{
    public string studentID { get; set; }
    public int value { get; set; }
    public float timeElapsedInSec { get; set; }
}
public class HighScoresUI : MonoBehaviour
{
    [SerializeField] DBManager dbManager;
    [SerializeField] Transform scoreParent;
    [SerializeField] GameObject scorePrefab;
    private List<Score> topScores;
    //private int lastRegionID;
    // Start is called before the first frame update
    private void Awake()
    {
        //lastRegionID = -1;
        topScores = new List<Score>();
    }
    private void OnEnable()
    {
        dbManager.GetTopScores(topScores);
        RenderScores();
    }

    private void OnDisable()
    {
        topScores.Clear();

        foreach (Transform child in scoreParent.transform)
        {
            Destroy(child.gameObject);
        }
    }
    private void RenderScores()
    {
        for (int i = 0; i < topScores.Count; i++)
        {
            GameObject tempObj = Instantiate(scorePrefab);

            tempObj.GetComponent<HighScorePrefabScript>().SetScoreProperties(i + 1, topScores[i].studentID, 
                topScores[i].value, topScores[i].timeElapsedInSec);
            tempObj.transform.SetParent(scoreParent);

            tempObj.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        }
    }
}
