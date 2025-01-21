using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Data;
using Mono.Data.Sqlite;
using JetBrains.Annotations;
using System.Globalization;
using System.Data.Common;
using Mapbox.Utils;
using Mapbox.Examples;
using Mapbox.Unity.Map;

public class DBManager : MonoBehaviour
{

    [SerializeField] JSONDataLoader _JSONDataLoader;
    [SerializeField] GameObject RealMap;
    [SerializeField] CameraAim _cameraAim;
    private SpawnOnMap _spawnOnMap;
    private AbstractMap _abstractMap;
    private string connectionString;
    private int activeRegionID = 0;
    private string activeStudentID = "0036524001";
    public event EventHandler<bool> OnRegionsLoaded;

    private struct NonCachedCity
    {
        public string cityName;
        public Vector2d worldCoordinates;
    }

    private struct RegionSettings
    {
        public float latitude;
        public float longitude;
        public float zoom;
    }

    private List<string> regions;
    private List<NonCachedCity> nonCachedCitiesList;
    private List<RegionSettings> regionSettingsList;
    private void Awake()
    {
        regions = new List<string>();
        nonCachedCitiesList = new List<NonCachedCity>();
        regionSettingsList = new List<RegionSettings>();
        _spawnOnMap = RealMap.GetComponent<SpawnOnMap>();
        //RealMap.SetActive(false);
        connectionString = "URI=file:" + Application.streamingAssetsPath + "/EduGameN.db";
        LoadRegions();
    }
    void Start()
    {
        _abstractMap = RealMap.GetComponent<AbstractMap>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_abstractMap == null)
        {
            _abstractMap = RealMap.GetComponent<AbstractMap>();
        }
        //foreach(Transform child in RealMap.transform)
        //{
        //    Debug.Log(child.gameObject.name);
        //}
    }

    private void GetStudents()
    {
        IDbConnection dbConnection = new SqliteConnection(connectionString);
        dbConnection.Open();

        // Example query
        IDbCommand dbCommand = dbConnection.CreateCommand();
        string sqlQuery = "SELECT * FROM student";
        dbCommand.CommandText = sqlQuery;

        // Execute query and read results
        IDataReader reader = dbCommand.ExecuteReader();
        while (reader.Read())
        {
            //Debug.Log($"JMBAG: {reader.GetString(0)}, Person: {reader.GetString(1)} {reader.GetString(2)} "); // Replace with your column index or name
        }

        // Clean up
        reader.Close();
        dbCommand.Dispose();
        dbConnection.Close();
    }

    public void LoadRegions()
    {
        using (IDbConnection dbConnection = new SqliteConnection(connectionString))
        {
            dbConnection.Open();

            using (IDbCommand dbCommand = dbConnection.CreateCommand())
            {
                string sqlQuery = "SELECT * from region";

                dbCommand.CommandText = sqlQuery;
                //Debug.Log(sqlQuery);
                using (IDataReader reader = dbCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //RegionInfo regionInfo = new RegionInfo();
                        //regionInfo.regionID = reader.GetInt16(0);
                        //regionInfo.regionName = reader.GetString(1);
                        RegionSettings regSettings = new RegionSettings();
                        regSettings.longitude = reader.GetFloat(2);
                        regSettings.latitude = reader.GetFloat(3);
                        regSettings.zoom = reader.GetFloat(4);
                        regionSettingsList.Add(regSettings);
                        regions.Add(reader.GetString(1));

                    }
                    reader.Close();
                    //dbCommand.Dispose();
                    dbConnection.Close();
                }
            }
        }
        OnRegionsLoaded?.Invoke(this, true);
    }
    
    public void HandleRegionChange(int regionID)
    {
        Debug.Log("FETCHING TILES AFTER CHANGING REGION");

        RegionSettings regionSettings = regionSettingsList[regionID];
        //RegionSettings regionSettings = new RegionSettings();
        Vector2d latitudeLongitudeCenter = new Vector2d(regionSettings.latitude, regionSettings.longitude);
        Debug.Log($"REGION: {regions[regionID]}, (lat, long): {latitudeLongitudeCenter}, zoom: {regionSettings.zoom}");
        _abstractMap.SetCenterLatitudeLongitude(latitudeLongitudeCenter);
        _abstractMap.SetZoom(regionSettings.zoom);
        _abstractMap.UpdateMap();
    }
    public void LoadCities(int regionID, List<CityData> citiesList)
    {
        _JSONDataLoader.ReplaceCitiesParent();
        nonCachedCitiesList.Clear();
        using (IDbConnection dbConnection = new SqliteConnection(connectionString))
        {
            dbConnection.Open();

            using (IDbCommand dbCommand = dbConnection.CreateCommand())
            {
                string sqlQuery = "SELECT city.name, city.longitude, city.latitude, city.cached, city.locationX, city.locationY"
                    + " FROM region JOIN city ON region.regionID = city.regionID"
                    + $" WHERE city.regionID = {regionID}";
                dbCommand.CommandText = sqlQuery;
                //Debug.Log(sqlQuery);
                using (IDataReader reader = dbCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //Debug.Log($"{reader.GetString(0)}, {reader.GetFloat(1)}, {reader.GetFloat(2)}, {reader.GetInt32(3)}");
                        CityData city = new CityData();
                        city.name = reader.GetString(0);
                        city.cached = reader.GetInt32(3) != 0;
                        city.location = new Location();

                        //Debug.Log($"CITY: {city.name}, cached: {city.cached}");
                        if (city.cached == false)
                        {
                            //RealMap.SetActive(true);
                            NonCachedCity nonCachedCity = new NonCachedCity();
                            //Debug.Log($"{city.name}: ({reader.GetFloat(2)},{reader.GetFloat(1)})");
                            //LATITUDE SHOULD BE BEFORE LONGITUDE: FORMAT IS 'LATITUDE,LONGITUDE'
                            Vector2d cityGeoCoordinates = new Vector2d(reader.GetFloat(2), reader.GetFloat(1));
                            Vector2d realCoordinates = _spawnOnMap.ConvertGeoCoordinatesToWorldCoordinates(cityGeoCoordinates);
                            nonCachedCity.cityName = city.name;
                            nonCachedCity.worldCoordinates = realCoordinates;
                            Debug.Log($"{city.name}: ({nonCachedCity.worldCoordinates.x},{nonCachedCity.worldCoordinates.y})");
                            nonCachedCitiesList.Add(nonCachedCity);
                        }
                        else
                        {
                            city.location.x = reader.GetFloat(4);
                            city.location.y = reader.GetFloat(5);
                        }
                        
                        citiesList.Add(city);
                    }
                    reader.Close();
                }
            }
            foreach (CityData city in citiesList)
            {
                if (city.cached == false)
                {
                    NonCachedCity nCC = nonCachedCitiesList.Find(nonCachedCity => nonCachedCity.cityName == city.name);
                    Debug.Log($"NCC: {nCC.worldCoordinates}");
                    city.location.x = (float)nCC.worldCoordinates.x;
                    city.location.y = (float)nCC.worldCoordinates.y;
                    Debug.Log($"CITY LOCATION: {city.location.x}, {city.location.y}");
                    using (IDbCommand updateCommand = dbConnection.CreateCommand())
                    {
                        updateCommand.CommandText = $"UPDATE city SET locationX = {city.location.x}, "
                            + $"locationY = {city.location.y}, cached = 1 WHERE name = '{city.name}'";
                        updateCommand.ExecuteNonQuery();
                    }
                }
                _JSONDataLoader.InstantiateCity(city);
            }
            dbConnection.Close();
            //RealMap.SetActive(false);
        }
    }

    public void AddScore(/*string studentID, int regionID,*/ int value, float elapsedTimeInSec)
    {
        using (IDbConnection dbConnection = new SqliteConnection(connectionString))
        {
            dbConnection.Open();

            using (IDbCommand dbCommand = dbConnection.CreateCommand())
            {
                string sqlQuery = "INSERT INTO score (value, timeElapsedInSec, studentJMBAG, regionID)"
                    + $" VALUES ({value}, {elapsedTimeInSec}, '{activeStudentID}', {activeRegionID})";
                dbCommand.CommandText = sqlQuery;
                dbCommand.ExecuteScalar();
            }
            dbConnection.Close();
        }
    }

    public int GetHighScore(/*string studentID, int regionID*/)
    {
        int highScore = 0;
        using (IDbConnection dbConnection = new SqliteConnection(connectionString))
        {
            dbConnection.Open();

            using (IDbCommand dbCommand = dbConnection.CreateCommand())
            {
                string sqlQuery = "SELECT MAX(value) as highscore FROM student JOIN score ON studentJMBAG = JMBAG"
                    + $" WHERE JMBAG = '{activeStudentID}' AND regionID = {activeRegionID}" +
                    " GROUP BY JMBAG";

                dbCommand.CommandText = sqlQuery;
                //Debug.Log(sqlQuery);
                using (IDataReader reader = dbCommand.ExecuteReader())
                {
                    //HIGHSCORE CAN'T BE ASSIGNED outside of while loop, even though only 1 value is being 
                    //retreived from database
                    while (reader.Read()) 
                    {
                        highScore = reader.GetInt16(0);
                        Debug.Log("results");
                        //Debug.Log(reader.GetInt16(0));
                    }
                    //highScore = reader.GetInt32(0);
                    //Debug.Log("highscore: " + highScore);
                    reader.Close();
                    //dbCommand.Dispose();
                    dbConnection.Close();
                }
            }
        }
        return highScore;
    }
    public void GetTopScores(/*string studentID, int regionID, *int scoreCount*/List<Score> scoresList)
    {
        using (IDbConnection dbConnection = new SqliteConnection(connectionString))
        {
            dbConnection.Open();

            using (IDbCommand dbCommand = dbConnection.CreateCommand())
            {
                string sqlQuery = "SELECT score.studentJMBAG, score.value, score.timeElapsedInSec" +
                    " FROM score JOIN region ON score.regionID = region.regionID"
                    + $" WHERE score.studentJMBAG = '{activeStudentID}' AND region.regionID = {activeRegionID}" +
                    " ORDER BY score.value DESC, score.timeElapsedInSec ASC"
                    + " LIMIT 10";

                dbCommand.CommandText = sqlQuery;
                //Debug.Log(sqlQuery);

                using (IDataReader reader = dbCommand.ExecuteReader())
                {
                    //Debug.Log(reader.FieldCount);
                    while (reader.Read())
                    {
                        //Debug.Log("in reader");
                        //Debug.Log($"Student: {reader.GetString(1)} {reader.GetString(2)} made score: {reader.GetInt16(3)} in {reader.GetFloat(4)} sec");
                        Score score = new Score();
                        score.studentID = reader.GetString(0);
                        score.value = reader.GetInt32(1);
                        score.timeElapsedInSec = reader.GetFloat(2);
                        scoresList.Add(score);
                    }
                    reader.Close();
                    //dbCommand.Dispose();
                    dbConnection.Close();

                }
            }
        }
    }

    public List<string> GetRegions()
    {
        return regions;
    }

    public int GetActiveRegionID()
    {
        return activeRegionID;
    }

    public void SetActiveRegionID(int regionID)
    {
        activeRegionID = regionID;
        StartCoroutine(_cameraAim.AlignCameraToMapDimensions());
    }
}
