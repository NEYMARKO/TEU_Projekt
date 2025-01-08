using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Data;
using Mono.Data.Sqlite;

public class DBManager : MonoBehaviour
{

    private string connectionString;

    // Start is called before the first frame update
    void Start()
    {
        //connectionString = "URI=file:" + Application.dataPath + "/StreamingAssets/EduGameN.sqlite";
        connectionString = "URI=file:" + Application.streamingAssetsPath + "/EduGameN.db";
        GetStudents();
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
            Debug.Log($"JMBAG: {reader.GetString(0)}, Person: {reader.GetString(1)} {reader.GetString(2)} "); // Replace with your column index or name
        }

        // Clean up
        reader.Close();
        dbCommand.Dispose();
        dbConnection.Close();
    }
}
