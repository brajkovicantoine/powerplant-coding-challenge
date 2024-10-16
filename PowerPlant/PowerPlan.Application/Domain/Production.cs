using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PowerPlant.Application.Domain;

public class Production
{
    [Required]
    [JsonPropertyName("load")]
    [Range(0, int.MaxValue)]
    public int Load { get; set; }

    [Required]
    [JsonPropertyName("fuels")]
    public required Fuel Fuel { get; set; }

    [Required]
    [JsonPropertyName("powerplants")]
    public required IEnumerable<PowerPlant> PowerPlants { get; set; }
}
