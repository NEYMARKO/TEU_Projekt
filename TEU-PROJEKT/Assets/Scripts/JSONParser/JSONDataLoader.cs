using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using System.IO;
public class CityData
{
    public string name;
    public Location location;
}

public class Location
{
    public float x;
    public float y;
}

public class AllCitiesInfo
{
    public List<CityData> cities;
}

public class JSONDataLoader : MonoBehaviour
{
    string jsonPath = "/Data/CitiesData.json";


    public GameObject cityPrefab;
    public GameObject citiesParentObject;

    [SerializeField] Transform map;

    private List<CityData> citiesData;
    private bool citiesLoaded = false;
    private bool failedToLoad = false;
    
    float mapYValue;
    void Start()
    {
        string fullPath = Application.dataPath + jsonPath;
        mapYValue = map.transform.position.y;
        citiesData = new List<CityData>();
        //Debug.Log(Application.dataPath + jsonPath);
        //AllCities allCities = JsonUtility.FromJson<AllCities>(Application.dataPath + jsonPath, );
        //Debug.Log(allCities);

        if (File.Exists(fullPath))
        {
            // Read the JSON file content
            string jsonString = File.ReadAllText(fullPath);

            //Debug.Log(jsonString);
            // Parse JSON into CityData
            AllCitiesInfo allCitiesInfo = JsonConvert.DeserializeObject<AllCitiesInfo>(jsonString);

            //Debug.Log(allCitiesInfo.cities);
            if (allCitiesInfo != null && allCitiesInfo.cities != null)
            {
                foreach (CityData city in allCitiesInfo.cities)
                {
                    citiesData.Add(city);

                    // Calculate position for the cube (z = 0)
                    Vector3 position = new Vector3(city.location.x, 0, city.location.y);

                    // Instantiate a cube at the given position
                    GameObject cityObject = Instantiate(cityPrefab, position, Quaternion.identity);

                    // Set the name of the cube to the city's name
                    cityObject.name = city.name;
                    cityObject.layer = LayerMask.NameToLayer("Map");
                    cityObject.transform.parent = citiesParentObject.transform;
                }
                Debug.Log("CITIES LOADED FINALLY");
                citiesLoaded = true;
            }
            else
            {
                Debug.LogError("Failed to parse JSON or no cities found!");
                failedToLoad = true;
            }
        }
        else
        {
            Debug.LogError($"JSON file not found at path: {fullPath}");
            failedToLoad = true;
        }
    }

    public List<CityData> ProvideCitiesInfo()
    {
        //Debug.Log("LIST");
        //foreach (CityData city in citiesData)
        //{
        //    Debug.Log(city.name);
        //}
        return citiesData;
    }

    public bool FailedToPopulateCitiesList()
    {
        return failedToLoad;
    }

    public bool CitiesListPopulated()
    {
        return citiesLoaded;
    }
}
