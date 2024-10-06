using System;
using System.Linq;
using UniCSV;
using UnityEngine;

public class CSVExample : MonoBehaviour
{
    [SerializeField] private TextAsset _usernamesCSV;
    [SerializeField] private TextAsset _csvWithEmptyLines;

    public class User
    {
        public string Username { get; set; }

        [CsvColumn(" Identifier")]
        public int Identifier { get; set; }

        [CsvColumn("First name")]
        public string FirstName { get; set; }

        [CsvColumn("Last name")]
        public string LastName { get; set; }
    }

    private void Start()
    {
        LoadCSV(_usernamesCSV);
        LoadCSV(_csvWithEmptyLines);

        var users = CsvParser.ParseFromString<User>(_usernamesCSV.text).ToList();
    }

    private void LoadCSV(TextAsset usernamesCsv)
    {
        try
        {
            var data = CsvParser.ParseFromString(usernamesCsv.text);

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