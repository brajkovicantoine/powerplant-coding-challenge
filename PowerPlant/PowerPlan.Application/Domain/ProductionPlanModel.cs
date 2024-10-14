namespace PowerPlant.Application.Domain;

public class ProductionPlanModel
{
    public required PowerPlant PowerPlant { get; set; }

    public decimal Production { get; set; }
}
