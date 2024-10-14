using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PowerPlant.Application.Domain;

public class Fuel
{
    [Required]
    [JsonPropertyName("gas(euro/MWh)")]
    public decimal GasPricePerMWh { get; set; }

    [Required]
    [JsonPropertyName("kerosine(euro/MWh)")]
    public decimal KerosinePricePerMWh { get; set; }

    [Required]
    [JsonPropertyName("co2(euro/ton)")]
    public decimal Co2PricePerTon { get; set; }

    [Required]
    [JsonPropertyName("wind(%)")]
    public decimal WindPerCent { get; set; }


    public decimal GetPrice(PowerType type)
    {
        return type switch
        {
            PowerType.GasFired => GasPricePerMWh,
            PowerType.Turbojet => KerosinePricePerMWh,
            PowerType.Windturbine => decimal.Zero,
            _ => throw new NotSupportedException(),
        };
    }
}
