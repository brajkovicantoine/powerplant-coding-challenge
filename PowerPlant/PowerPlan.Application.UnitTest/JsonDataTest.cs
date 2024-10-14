using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PowerPlan.Application.UnitTest;

public class JsonDataTest<T>
{
    // The relative file path to the JSON file containing test data.
    public string FilePath { get; }

    private static JsonSerializerOptions SerializerOptions { get; } = CreateDefaultSerializerOptions();

    private static JsonSerializerOptions CreateDefaultSerializerOptions()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new JsonStringEnumConverter());

        return options;
    }

    // Constructor that takes the path to the JSON file as an argument.
    public JsonDataTest(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
        }

        FilePath = filePath;
    }

    // Reads the test data from the JSON file, deserializes it, and returns it as an enumerable of object arrays.
    // This allows the test data to be easily consumed by Xunit's [Theory] tests.
    public T GetData()
    {
        // Gets the executing assembly's location and derives the solution directory.
        // This is intended to find the JSON file in a location relative to the solution directory.
        string assemblyLocation = Assembly.GetExecutingAssembly().Location;

        string assemblyDirectory = Path.GetDirectoryName(assemblyLocation)!;

        string solutionDirectory = Path.GetFullPath(Path.Combine(assemblyDirectory, @"..\..\.."));

        var pathToReadFrom = Path.Combine(solutionDirectory, FilePath);
        using var textReader = File.OpenRead(pathToReadFrom);
        
        return JsonSerializer.Deserialize<T>(textReader, SerializerOptions);
    }
}
