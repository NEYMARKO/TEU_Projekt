using JetBrains.Annotations;
using Mapbox.Unity.Map;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class GameLoop : MonoBehaviour
{

    [SerializeField] CameraAim _cameraAim;
    [SerializeField] GameObject citiesParent;
    [Header("UI")]
    [SerializeField] GameObject endGameUIHolder;
    [SerializeField] GameObject pauseMenuUIHolder;
    [Header("Data")]
    [SerializeField] JSONDataLoader _JSONDataLoader;
    [Header("Database")]
    [SerializeField] DBManager databaseManager;
    [SerializeField] AbstractMap _abstractMap;
    private List<CityData> shuffledCities;
    private List<CityData> allCities;

    public event EventHandler<bool> OnGameEnd;
    public event EventHandler<bool> OnGameStart;

    bool gameEndedRegistered = false;

    bool canFetchNext = true;
    string currentWantedCity;
    int correctlyGuessed = 0;
    private float elapsedTime = 0f;
    private bool pauseTime = false;
    private int highScore = -1;

    private bool citiesLoaded = false;
    void Start()
    {
        
        allCities = new List<CityData>();
        //Debug.Log("BEFORE LOADING");
        if (_abstractMap != null)
        {
            databaseManager.LoadCities(databaseManager.GetActiveRegionID(), allCities);
            shuffledCities = ShuffleList(allCities);
            citiesLoaded = true;
        }
        StartCoroutine(_cameraAim.AlignCameraToMapDimensions());
    }

    void Update()
    {
        if (!citiesLoaded && _abstractMap != null)
        {
            //StartCoroutine(_cameraAim.AlignCameraToMapDimensions());
            databaseManager.LoadCities(databaseManager.GetActiveRegionID(), allCities);
            citiesLoaded = true;
        }
        FetchNextCity();
        if (Input.GetKeyDown(KeyCode.Escape)) TogglePauseMenu();
        if (!pauseMenuUIHolder.activeSelf && !pauseTime) elapsedTime += Time.deltaTime;
    }

    private List<CityData> ShuffleList(List<CityData> originalListData)
    {
        List<CityData> shuffledListData = Enumerable.Repeat<CityData>(null, originalListData.Count).ToList();
        int position;

        for (int i = 0; i < originalListData.Count; i++)
        {
            position = UnityEngine.Random.Range(0, originalListData.Count);
            // element already placed at that position in list, must find different position
            while (shuffledListData[position] != null)
            {
                position = UnityEngine.Random.Range(0, originalListData.Count);
            }
            shuffledListData[position] = originalListData[i];
        }
        return shuffledListData;
    }
    private void FetchNextCity()
    {
        if (!canFetchNext) return;
        if (gameEndedRegistered) gameEndedRegistered = false;
        if (shuffledCities.Count == 0)
        {
            //Debug.Log("SHUFFLED CITIES are GONE");
            CheckGameOver();
            return;
        }
        currentWantedCity = shuffledCities[0].name;
        shuffledCities.RemoveAt(0);
        canFetchNext = false;
        return;
    }
    private void CheckGameOver()
    {
        if (shuffledCities.Count == 0 && !gameEndedRegistered)
        {
            gameEndedRegistered = true;
            foreach (Transform city in _JSONDataLoader.GetTempParentObj().transform)
            {
                city.gameObject.GetComponent<CityBehaviour>().ColorCityAsIncorrect();
            }
            pauseTime = true;
            databaseManager.AddScore(correctlyGuessed, (Mathf.Floor(elapsedTime * 100) / 100));
            StartCoroutine(Wait());
            canFetchNext = false;
            return;
        }
    }

    public bool CheckCityGuess(string cityGuess)
    {
        canFetchNext = true;

        if (currentWantedCity == cityGuess)
        {
            correctlyGuessed++;
            return true;
        }
        return false;
    }

    public void RestartGame(GameObject uiCaller)
    {
        shuffledCities = ShuffleList(allCities);
        Debug.Log($"Shuffled cities count: {shuffledCities.Count}");
        canFetchNext = true;
        correctlyGuessed = 0;
        elapsedTime = 0f;
        pauseTime = false;
        highScore = -1;
        ResetCities();
        //OnGameStart?.Invoke(this, true);
        StartCoroutine(_cameraAim.AlignCameraToMapDimensions());
        uiCaller.SetActive(false);
    }

    public void ResumeGame(GameObject uiCaller)
    {
        uiCaller.SetActive(false);
    }

    public void ReloadLevel(int regionID, GameObject uiCaller)
    {
        allCities.Clear();
        databaseManager.LoadCities(regionID, allCities);
        RestartGame(uiCaller);
    }
    private void ResetCities()
    { 
        foreach(Transform city in _JSONDataLoader.GetTempParentObj().transform)
        {
            city.gameObject.GetComponent<CityBehaviour>().ResetCity();
        }
    }

    public int GetCorrectAnswersCount()
    {
        return correctlyGuessed;
    }

    public int GetHighScore()
    {
        if (highScore == -1) highScore = databaseManager.GetHighScore();
        return highScore;
    }

    public int GetCitiesCount()
    {
        return allCities.Count; 
    }
    public string GetWantedCity()
    {
        return currentWantedCity;
    }

    public float GetElapsedTime()
    {
        return Mathf.Floor(elapsedTime * 100)/100;
    }
    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(2);
        endGameUIHolder.SetActive(true);
    }

    private void TogglePauseMenu()
    {
        pauseMenuUIHolder.SetActive(!pauseMenuUIHolder.activeSelf);
    }
}
