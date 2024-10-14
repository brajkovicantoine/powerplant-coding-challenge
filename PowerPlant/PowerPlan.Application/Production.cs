using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PowerPlan.Application;

public class ProductionPlan
{
    [Required]
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [Required]
    [JsonPropertyName("p")]
    public decimal Production { get; set; }
}

public class Production
{
    [Required]
    [JsonPropertyName("load")]
    public decimal Load { get; set; }

    [Required]
    [JsonPropertyName("fuels")]
    public required Fuel Fuel { get; set; }

    [Required]
    [JsonPropertyName("powerplants")]
    public required IEnumerable<PowerPlant> PowerPlants { get; set; }
}

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

public enum PowerType
{
    GasFired = 1,
    Turbojet =2,
    Windturbine = 3
}
