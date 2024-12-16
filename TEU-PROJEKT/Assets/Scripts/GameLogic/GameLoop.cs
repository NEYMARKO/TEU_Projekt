using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameLoop : MonoBehaviour
{
    [SerializeField] GameObject citiesParent;
    [Header("UI")]
    [SerializeField] GameObject endGameUIHolder;
    [Header("Data")]
    [SerializeField] JSONDataLoader _JSONDataLoader;
    private List<CityData> shuffledCities;
    private List<CityData> allCities;

    bool canFetchNext = true;
    string currentWantedCity;
    int correctlyGuessed = 0;
    void Start()
    {
        //wait until all cities are populated in list
        //while (!_JSONDataLoader.CitiesListPopulated() && !_JSONDataLoader.FailedToPopulateCitiesList())
        //{
        //    Debug.Log("CITIES LIST POPULATED: " + _JSONDataLoader.CitiesListPopulated());
        //}
        
        allCities = _JSONDataLoader.ProvideCitiesInfo();
        shuffledCities = ShuffleList(allCities);
        //Debug.Log("SHUFFLED LIST");
        //foreach (CityData city in allCities)
        //{
        //    Debug.Log(city.name);
        //}
    }

    // Update is called once per frame
    void Update()
    {
        FetchNextCity();
    }

    private List<CityData> ShuffleList(List<CityData> originalListData)
    {
        //Debug.Log("ORIGINAL LIST");
        //foreach (CityData city in originalListData)
        //{
        //    Debug.Log(city.name);
        //}
        List<CityData> shuffledListData = Enumerable.Repeat<CityData>(null, originalListData.Count).ToList();
        int position;

        //Debug.Log($"ORIG COUNT: {originalListData.Count}, SHUFF COUNT: {shuffledListData.Count}");
        for (int i = 0; i < originalListData.Count; i++)
        {
            position = Random.Range(0, originalListData.Count);
            // element already placed at that position in list, must find different position
            while (shuffledListData[position] != null)
            {
                position = Random.Range(0, originalListData.Count);
            }
            shuffledListData[position] = originalListData[i];
        }
        return shuffledListData;
    }
    private void FetchNextCity()
    {
        if (!canFetchNext) return;
        else if (shuffledCities.Count == 0)
        {
            CheckGameOver();
            return;
        }
        currentWantedCity = shuffledCities[0].name;
        shuffledCities.RemoveAt(0);
        //Debug.Log($"            Looking for: {currentWantedCity}");
        canFetchNext = false;
        return;
    }
    private void CheckGameOver()
    {
        if (shuffledCities.Count == 0)
        {
            //Debug.Log($"GAME OVER, correct guesses: {correctlyGuessed}");
            StartCoroutine(Wait());
            canFetchNext = false;
            return;
        }
    }

    public bool CheckCityGuess(string cityGuess)
    {
        //Debug.Log($"SELECTED: {cityGuess}");
        canFetchNext = true;

        if (currentWantedCity == cityGuess)
        {
            correctlyGuessed++;
            return true;
        }
        return false;
    }

    public void RestartGame()
    {
        shuffledCities = ShuffleList(allCities);
        canFetchNext = true;
        correctlyGuessed = 0;
        ResetCities();
        endGameUIHolder.SetActive(false);
    }

    private void ResetCities()
    { 
        foreach(Transform city in citiesParent.transform)
        {
            city.gameObject.GetComponent<CityBehaviour>().ResetCity();
        }
    }
    public int GetCorrectAnswersCount()
    {
        return correctlyGuessed;
    }

    public int GetCitiesCount()
    {
        return allCities.Count; 
    }
    public string GetWantedCity()
    {
        return currentWantedCity;
    }
    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(2);
        endGameUIHolder.SetActive(true);
    }
}
