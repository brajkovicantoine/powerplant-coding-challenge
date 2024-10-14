using MediatR;
using PowerPlant.Application.Domain;

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
        var plan = CalculateProductionPlan(request.Production);
        plan = RetroFitProductionPlan(plan, request.Production);
        return Task.FromResult(plan.Select(x => new ProductionPlan() { Name = x.PowerPlant.Name, Production = x.Production }));
    }

    private static IEnumerable<ProductionPlanModel> CalculateProductionPlan(Production production)
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
                yield return new ProductionPlanModel() { PowerPlant = powerPlant, Production = 0 };
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
                yield return new ProductionPlanModel() { PowerPlant = powerPlant, Production = prod/10m };
                continue;
            }

            //left load is in range
            yield return new ProductionPlanModel() { PowerPlant = powerPlant, Production = (loadLeftToProduce/10m) };
            loadLeftToProduce -= loadLeftToProduce;
        }
    }

    private static IEnumerable<ProductionPlanModel> RetroFitProductionPlan(IEnumerable<ProductionPlanModel> plans, Production production)
    {
        if (production.Load == plans.Sum(p => p.Production))
            return plans;

        var newPlans = plans.ToList();
        var lastProd = newPlans.LastOrDefault(x => x.Production != 0);
        if (lastProd == null)
            return plans;
        lastProd.Production = 0;
        newPlans.Reverse();

        foreach (var planItem in newPlans)
        {
            if (newPlans.Sum(p => p.Production) == production.Load)
                continue;//retrofit achieved

            planItem.Production -= Math.Min(planItem.Production, newPlans.Sum(p => p.Production) - production.Load);
            planItem.Production = Math.Min(planItem.Production, planItem.PowerPlant.ProductionMinimal);
        }
        return newPlans;
    }
}
