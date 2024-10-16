using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PowerPlant.Application.Domain;

public class Fuel
{
    [Required]
    [JsonPropertyName("gas(euro/MWh)")]
    [Range(0, int.MaxValue)]
    public decimal GasPricePerMWh { get; set; }

    [Required]
    [JsonPropertyName("kerosine(euro/MWh)")]
    [Range(0, int.MaxValue)]
    public decimal KerosinePricePerMWh { get; set; }

    [Required]
    [JsonPropertyName("co2(euro/ton)")]
    [Range(0, int.MaxValue)]
    public decimal Co2PricePerTon { get; set; }

    [Required]
    [JsonPropertyName("wind(%)")]
    [Range(0, 100)]
    public decimal WindPerCent { get; set; }
}
