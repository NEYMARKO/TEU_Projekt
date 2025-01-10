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

    private string connectionString;

    private int activeRegionID = 0;
    private string activeStudentID = "0036524001";

    //private struct RegionInfo
    //{
    //    public int regionID;
    //    public string regionName;
    //}

    //private List<RegionInfo> regions;
    private List<string> regions;

    void Start()
    {
        //regions = new List<RegionInfo>();
        regions = new List<string>();
        //connectionString = "URI=file:" + Application.dataPath + "/StreamingAssets/EduGameN.sqlite";
        connectionString = "URI=file:" + Application.streamingAssetsPath + "/EduGameN.db";
        //GetStudents();
        GetTopScores(/*"0036524001", 0, */10);
        LoadRegions();

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
    private void GetTopScores(/*string studentID, int regionID, */int scoreCount)
    {
        using (IDbConnection dbConnection = new SqliteConnection(connectionString))
        {
            dbConnection.Open();

            using (IDbCommand dbCommand = dbConnection.CreateCommand())
            {
                string sqlQuery = "SELECT * FROM student JOIN score ON studentJMBAG = JMBAG"
                    + $" WHERE JMBAG = '{activeStudentID}' AND regionID = {activeRegionID}" +
                    " ORDER BY value DESC, timeElapsedInSec ASC";
                //string sqlQuery = "select * from student join score on studentJMBAG = JMBAG WHERE JMBAG = '0036524001' AND regionID = 0 order by value desc, timeElapsedInSec asc";
                //string sqlQuery = "select * from student join score on studentJMBAG = JMBAG order by value desc, timeElapsedInSec asc";
                //string sqlQuery = "SELECT * FROM score ORDER BY value DESC, timeElapsedInSec ASC";
                //sqlQuery = "SELECT * FROM student";
                dbCommand.CommandText = sqlQuery;
                //Debug.Log(sqlQuery);

                using (IDataReader reader = dbCommand.ExecuteReader())
                {
                    //Debug.Log(reader.FieldCount);
                    while (reader.Read())
                    {
                        //Debug.Log("in reader");
                        //Debug.Log($"Student: {reader.GetString(1)} {reader.GetString(2)} made score: {reader.GetInt16(3)} in {reader.GetFloat(4)} sec");
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
}
