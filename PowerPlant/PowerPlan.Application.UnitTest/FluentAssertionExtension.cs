using FluentAssertions;
using FluentAssertions.Collections;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using PowerPlant.Application.Domain;

namespace PowerPlant.Application.UnitTest;

public static class ProductionPlanExtensions
{
    public static ProductionPlanAssertions Should(this ProductionPlan instance)
    {
        return new ProductionPlanAssertions(instance);
    }
}

public class ProductionPlanAssertions : ReferenceTypeAssertions<ProductionPlan, ProductionPlanAssertions>
{
    protected override string Identifier => "ProductionPlan";

    public ProductionPlanAssertions(ProductionPlan instance)
        : base(instance)
    {
    }

    [CustomAssertion]
    public AndConstraint<ProductionPlanAssertions> HaveProductionIsWithinBound(Production production, string because = "", params object[] becauseArgs)
    {
        var power = production.PowerPlants.FirstOrDefault(y => y.Name == Subject.Name);

        Execute.Assertion.
            BecauseOf(because, becauseArgs)
            .ForCondition(power is not null)
            .FailWith("No powerplant found for the name {0}", Subject.Name)
            .Then
            .ForCondition(Subject.Production == 0 || (Subject.Production >= power.ProductionMinimal && Subject.Production <= power.ProductionMaximal))
            .FailWith("Production value {0} is not inside the boundaries {1} {2} or equal to 0", Subject.Production, power.ProductionMinimal, power.ProductionMaximal);

        return new AndConstraint<ProductionPlanAssertions>(this);
    }
}

