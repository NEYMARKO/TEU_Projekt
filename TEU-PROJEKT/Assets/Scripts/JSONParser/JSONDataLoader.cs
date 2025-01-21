using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using System.IO;
public class CityData
{
    public string name;
    public Location location;
    public bool cached;
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
    private GameObject tempCitiesParentObj;
    //private GameObject citiesParentObjectReserve;
    //[SerializeField] Transform map;

    private List<CityData> citiesData;
    private bool citiesLoaded = false;
    private bool failedToLoad = false;
    
    float mapYValue;
    void Start()
    {
        tempCitiesParentObj = new GameObject();
        tempCitiesParentObj.transform.position = citiesParentObject.transform.position;
    }

    public void InstantiateCity(CityData city)
    {
        Vector3 position = new Vector3(city.location.x, 0, city.location.y);

        GameObject cityObject = Instantiate(cityPrefab, position, Quaternion.identity);

        cityObject.name = city.name;
        cityObject.layer = LayerMask.NameToLayer("Map");
        cityObject.transform.parent = tempCitiesParentObj.transform;

        return;
    }
    public List<CityData> ProvideCitiesInfo()
    {
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

    public void ReplaceCitiesParent()
    {
        Destroy(tempCitiesParentObj);
        tempCitiesParentObj = new GameObject();
        tempCitiesParentObj.transform.position = citiesParentObject.transform.position;
    }
    public GameObject GetTempParentObj()
    {
        return tempCitiesParentObj;
    }
}
