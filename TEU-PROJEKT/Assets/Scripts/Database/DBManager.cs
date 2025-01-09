using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Data;
using Mono.Data.Sqlite;
using JetBrains.Annotations;

public class DBManager : MonoBehaviour
{

    private string connectionString;

    private int activeRegion = 0;
    private int activeStudent = 0;


    void Start()
    {
        //connectionString = "URI=file:" + Application.dataPath + "/StreamingAssets/EduGameN.sqlite";
        connectionString = "URI=file:" + Application.streamingAssetsPath + "/EduGameN.db";
        GetStudents();
        GetTopScores("0036524001", 0, 10);
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

    private void GetTopScores(string studentID, int regionID, int scoreCount)
    {
        using (IDbConnection dbConnection = new SqliteConnection(connectionString))
        {
            dbConnection.Open();

            using (IDbCommand dbCommand = dbConnection.CreateCommand())
            {
                string sqlQuery = "SELECT * FROM student JOIN score ON studentJMBAG = JMBAG"
                    + $" WHERE JMBAG = '{studentID}' AND regionID = {regionID}" +
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
                        Debug.Log($"Student: {reader.GetString(1)} {reader.GetString(2)} made score: {reader.GetInt16(3)} in {reader.GetFloat(4)} sec");
                    }
                    reader.Close();
                    //dbCommand.Dispose();
                    dbConnection.Close();
                }
            }
        }
    }
    private void AddScore(int value, int timeElapsedInSec, int studentID, int regionID)
    {

    }
}
