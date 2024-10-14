using MediatR;

namespace PowerPlan.Application;

public class ProductionPlanRequest : IRequest<IEnumerable<ProductionPlan>>
{
    public Production Production { get; init; }

    public ProductionPlanRequest(Production production)
    {
        Production = production;
    }
}

public class ProductionPlanHandler : IRequestHandler<ProductionPlanRequest, IEnumerable<ProductionPlan>>
{
    public Task<IEnumerable<ProductionPlan>> Handle(ProductionPlanRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(ProductionPlanForGasAndKerosine(request.Production));
    }

    private static IEnumerable<ProductionPlan> ProductionPlanForGasAndKerosine(Production production)
    {
        var loadLeftToProduce = (int)production.Load * 10;
        var powerPlants = production.PowerPlants
            .OrderBy(x => x.GetMerit(production.Fuel.GetPrice(x.Type)))
            .ThenByDescending(x => x.ProductionMaximal)
            .ThenBy(x => x.Name);

        foreach (var powerPlant in powerPlants)
        {
            //the minimal value to produce is greater than the left load
            if ((powerPlant.ProductionMinimal*10) > loadLeftToProduce)
            {
                yield return new ProductionPlan() { Name = powerPlant.Name, Production = 0 };
                continue;
            }

            var prod = (int)powerPlant.ProductionMaximal*10;
            if (powerPlant.Type == PowerType.Windturbine)
            {
                prod = (int)(prod * (production.Fuel.WindPerCent / 100));
            }

            //the maximal value to produce is less than the left load
            if (prod <= loadLeftToProduce)
            {
                loadLeftToProduce -= prod;
                yield return new ProductionPlan() { Name = powerPlant.Name, Production = prod/10m };
                continue;
            }

            //left load is in range
            yield return new ProductionPlan() { Name = powerPlant.Name, Production = (loadLeftToProduce/10m) };
            loadLeftToProduce -= loadLeftToProduce;
        }
    }
}
