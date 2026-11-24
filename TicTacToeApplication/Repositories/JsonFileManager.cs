using System.Text.Json;
using System.Text.Json.Serialization;

namespace TicTacToe.Repositories;

/// <summary>
/// Contains the Write and load logics that writes and loads the file in json format.
/// </summary>
public class JsonFileManager
{
    private readonly JsonSerializerOptions _options = new JsonSerializerOptions
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() },
    };

    /// <summary>
    /// Writes all the data to the file.
    /// </summary>
    /// <typeparam name="T">The type parameter of the data object to be written.</typeparam>
    /// <param name="filePath">The path of the file where the data are stored.</param>
    /// <param name="list">List of the data that are to be added.</param>
    public void WriteAll<T>(string filePath, List<T> list)
    {
        string json = JsonSerializer.Serialize(list, this._options);
        File.WriteAllText(filePath, json);
    }

    /// <summary>
    /// Loads all the content and loads into the file.
    /// </summary>
    /// <typeparam name="T">The type parameter of the data to read from the file.</typeparam>
    /// <param name="filePath">Path of the file from which the contents are loaded. </param>
    /// <returns>A list of data that are stored in the file.</returns>
    public List<T> LoadAll<T>(string filePath)
    {
        try
        {
            string text = File.ReadAllText(filePath);
            if (string.IsNullOrWhiteSpace(text))
            {
                return new List<T>();
            }

            return JsonSerializer.Deserialize<List<T>>(text, this._options) ?? new List<T>();
        }
        catch (Exception ex)
        {
            throw new FileLoadException("Failed to load the data history - Please try again", ex);
        }
    }
}
