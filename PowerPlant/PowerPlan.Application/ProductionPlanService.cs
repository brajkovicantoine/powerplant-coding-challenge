using PowerPlant.Application.Domain;

namespace PowerPlan.Application;

public interface IProductionPlanService
{
    Task<IEnumerable<ProductionPlan>> CalculateProductionPlan(Production request, CancellationToken cancellationToken);
}

public class ProductionPlanService : IProductionPlanService
{
    public Task<IEnumerable<ProductionPlan>> CalculateProductionPlan(Production request, CancellationToken cancellationToken)
    {
        var plan = CalculateProductionPlan(request);
        plan = RetroFitProductionPlan(plan, request);
        return Task.FromResult(plan.Select(x => new ProductionPlan() { Name = x.PowerPlant.Name, Production = x.Production }));
    }

    private IEnumerable<ProductionPlanModel> CalculateProductionPlan(Production production)
    {
        var loadLeftToProduce = (decimal)production.Load;
        var powerPlants = production.PowerPlants
            .OrderBy(x => x.GetMerit(production.Fuel))
            .ThenByDescending(x => x.ProductionMaximal);

        //load with the maximal value to maximize the load
        foreach (var powerPlant in powerPlants)
        {
            //the minimal value to produce is greater than the left load
            if (powerPlant.ProductionMinimal > loadLeftToProduce)
            {
                yield return new ProductionPlanModel() { PowerPlant = powerPlant, Production = 0 };
                continue;
            }

            var prod = decimal.Min(powerPlant.GetMaximalBoundedProduction(production.Fuel), loadLeftToProduce);
            prod = decimal.Round(prod, 1, MidpointRounding.ToZero);

            //the maximal value to produce is less than the left load
            yield return new ProductionPlanModel() { PowerPlant = powerPlant, Production = prod };
            loadLeftToProduce -= prod;
        }
    }

    private IEnumerable<ProductionPlanModel> RetroFitProductionPlan(IEnumerable<ProductionPlanModel> plans, Production production)
    {
        var sumLoad = plans.Sum(p => p.Production);
        //no overload or underload
        if (sumLoad == production.Load)
            return plans;

        var newPlans = plans.ToList();

        //all production to 0 so we can't retrofit 
        //throw exception ? 
        if (sumLoad == 0)
            return plans;

        //we try to fit the remaining load in reverse
        newPlans.Reverse();
        ProductionPlanModel? lastProd = null;

        foreach (var planItem in newPlans)
        {
            var diffToRetroFit = newPlans.Sum(p => p.Production) - production.Load;

            var prod = decimal.Min(planItem.Production - diffToRetroFit, planItem.PowerPlant.GetMaximalBoundedProduction(production.Fuel));
            prod = decimal.Max(prod, planItem.PowerPlant.ProductionMinimal);
            planItem.Production = decimal.Round(prod, 1, MidpointRounding.ToZero);

            //check if the current can hold the load of the previous
            if (lastProd != null && lastProd.Production+planItem.Production <= planItem.PowerPlant.GetMaximalBoundedProduction(production.Fuel))
            {
                planItem.Production += lastProd.Production;
                lastProd.Production = 0;
            }

            lastProd = planItem;
        }

        //overload or underload throw exception ? or we send the best computed for now
        if (plans.Sum(p => p.Production) != production.Load)
            return newPlans;

        return newPlans;
    }
}
