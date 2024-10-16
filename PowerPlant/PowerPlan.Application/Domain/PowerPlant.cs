﻿using System.ComponentModel.DataAnnotations;
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
    [Range(0, 100)]
    public decimal Efficiency { get; set; }

    [Required]
    [JsonPropertyName("pmin")]
    [Range(0, int.MaxValue)]
    public decimal ProductionMinimal { get; set; }

    [Required]
    [JsonPropertyName("pmax")]
    [Range(0, int.MaxValue)]
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

    public decimal GetMaximalBoundedProduction(Fuel fuel)
    {
        var max = Type switch
        {
            PowerType.GasFired => ProductionMaximal,
            PowerType.Turbojet => ProductionMaximal,
            PowerType.Windturbine => ProductionMaximal * (fuel.WindPerCent / 100),
            _ => throw new NotSupportedException(),
        };
        return decimal.Max(ProductionMinimal, max);
    }
}
