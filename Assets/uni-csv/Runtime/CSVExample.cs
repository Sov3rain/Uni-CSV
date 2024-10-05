using System;
using UnityEngine;

public class CSVExample : MonoBehaviour
{
    [SerializeField] private TextAsset _usernamesCSV;
    [SerializeField] private TextAsset _csvWithEmptyLines;

    private void Start()
    {
        LoadCSV(_usernamesCSV);
        LoadCSV(_csvWithEmptyLines);
    }

    private void LoadCSV(TextAsset usernamesCsv)
    {
        try
        {
            var data = CSVParser.ParseFromString(usernamesCsv.text);

            Debug.Log($"Loaded {data.Count} rows from file");

            foreach (var row in data)
            {
                string rowContent = string.Join(", ", row);
                Debug.Log($"Row: {rowContent}");
            }
            
            Debug.Log("====================================");
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            throw;
        }
    }
}