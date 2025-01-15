using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    [SerializeField] TextMeshProUGUI rankTextObj;
    [SerializeField] TextMeshProUGUI scoreTextObj;
    [SerializeField] TextMeshProUGUI timeTextObj;

    private GameObject languageController;
    private JSONMenuLoader menuContentLoader;
    private List<Score> topScores;
    private string currentLanguage = "";
    private HighScoreTable currentHighScoreTableContent;
    //private int lastRegionID;
    // Start is called before the first frame update
    private void Awake()
    {
        //lastRegionID = -1;
        topScores = new List<Score>();
        languageController = GameObject.Find("LanguageController");
        if (languageController == null)
        {
            Debug.LogError("LanguageController not found in the scene!");
        }
        else
        {
            menuContentLoader = languageController.GetComponent<JSONMenuLoader>();
        }
    }
    private void OnEnable()
    {
        dbManager.GetTopScores(topScores);
        if (currentLanguage != menuContentLoader.currentLanguage)
        {
            currentLanguage = menuContentLoader.currentLanguage;
            currentHighScoreTableContent = menuContentLoader.GetHighScoreTableContent(currentLanguage);
            HandleLanguageChange();
        }
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

    private void HandleLanguageChange()
    {
        rankTextObj.text = currentHighScoreTableContent.rank.ToUpper();
        scoreTextObj.text = currentHighScoreTableContent.score.ToUpper();
        timeTextObj.text = currentHighScoreTableContent.time.ToUpper();
    }
}
