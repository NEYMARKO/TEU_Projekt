using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Data;
using Mono.Data.Sqlite;
using JetBrains.Annotations;
using System.Globalization;

public class DBManager : MonoBehaviour
{

    [SerializeField] JSONDataLoader _JSONDataLoader;
    private string connectionString;
    private int activeRegionID = 0;
    private string activeStudentID = "0036524001";
    public event EventHandler<bool> OnRegionsLoaded;
    //private struct RegionInfo
    //{
    //    public int regionID;
    //    public string regionName;
    //}

    //private List<RegionInfo> regions;
    private List<string> regions;

    private void Awake()
    {
        regions = new List<string>();
        connectionString = "URI=file:" + Application.streamingAssetsPath + "/EduGameN.db";
        LoadRegions();
    }
    void Start()
    {
        //regions = new List<RegionInfo>();
        //regions = new List<string>();
        //connectionString = "URI=file:" + Application.dataPath + "/StreamingAssets/EduGameN.sqlite";
        //connectionString = "URI=file:" + Application.streamingAssetsPath + "/EduGameN.db";
        //GetStudents();
        //GetTopScores(/*"0036524001", 0, */10);
        //LoadRegions();

        //foreach (var region in regions)
        //{
        //    //Debug.Log($"REGION ID: {region.regionID}, REGION NAME: {region.regionName}");
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
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
    
    public void LoadCities(int regionID, List<CityData> citiesList)
    {
        _JSONDataLoader.ReplaceCitiesParent();
        using (IDbConnection dbConnection = new SqliteConnection(connectionString))
        {
            dbConnection.Open();

            using (IDbCommand dbCommand = dbConnection.CreateCommand())
            {
                string sqlQuery = "SELECT city.name, city.locationX, city.locationY, city.cached"
                    + " FROM region JOIN city ON region.regionID = city.regionID"
                    + $" WHERE city.regionID = {regionID}";
                dbCommand.CommandText = sqlQuery;
                //Debug.Log(sqlQuery);
                using (IDataReader reader = dbCommand.ExecuteReader())
                {
                    //Debug.Log(reader.FieldCount);
                    while (reader.Read())
                    {
                        //Debug.Log($"{reader.GetString(0)}, {reader.GetFloat(1)}, {reader.GetFloat(2)}, {reader.GetInt32(3)}");
                        CityData city = new CityData();
                        city.location = new Location();
                        city.name = reader.GetString(0);
                        city.location.x = reader.GetFloat(1);
                        city.location.y = reader.GetFloat(2);
                        city.cached = reader.GetInt32(3) != 0;
                        _JSONDataLoader.InstantiateCity(city);
                        citiesList.Add(city);
                    }
                    reader.Close();
                    //dbCommand.Dispose();
                    dbConnection.Close();
                }
            }
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
                dbConnection.Close();
            }
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
    }
}
