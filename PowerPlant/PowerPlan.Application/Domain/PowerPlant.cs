using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PowerPlant.Application.Domain;

public class PowerPlant
{
    [Required]
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [Required]
    [JsonPropertyName("type")]
    public required PowerType Type { get; set; }

    [Required]
    [JsonPropertyName("efficiency")]
    public decimal Efficiency { get; set; }

    [Required]
    [JsonPropertyName("pmin")]
    public decimal ProductionMinimal { get; set; }

    [Required]
    [JsonPropertyName("pmax")]
    public decimal ProductionMaximal { get; set; }

    public decimal GetMerit(decimal price)
    {
        if (Efficiency == 0)
            return decimal.MaxValue;
        return Type switch
        {
            PowerType.GasFired => price / Efficiency,
            PowerType.Turbojet => price / Efficiency,
            PowerType.Windturbine => decimal.Zero,
            _ => throw new NotSupportedException(),
        };
    }
}
