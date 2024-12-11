using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameLoop : MonoBehaviour
{
    [SerializeField] JSONDataLoader _JSONDataLoader;
    private List<CityData> allCities;
    void Start()
    {
        //wait until all cities are populated in list
        //while (!_JSONDataLoader.CitiesListPopulated() && !_JSONDataLoader.FailedToPopulateCitiesList())
        //{
        //    Debug.Log("CITIES LIST POPULATED: " + _JSONDataLoader.CitiesListPopulated());
        //}
        
        allCities = _JSONDataLoader.ProvideCitiesInfo();
        allCities = ShuffleList(allCities);
        Debug.Log("SHUFFLED LIST");
        foreach (CityData city in allCities)
        {
            Debug.Log(city.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
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
            while (shuffledListData[position] != null)
            {
                position = Random.Range(0, originalListData.Count);
            }
            shuffledListData[position] = originalListData[i];
        }
        return shuffledListData;
    }
}
