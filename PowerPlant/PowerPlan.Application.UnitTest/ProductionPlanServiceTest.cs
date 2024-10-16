using FluentAssertions;
using PowerPlant.Application.Domain;
using PowerPlant.Application.UnitTest;
namespace PowerPlan.Application.UnitTest;

[TestClass]
public class ProductionPlanServiceTest
{
    private readonly ProductionPlanService productionPlanService = new ProductionPlanService();

    [TestMethod]
    public async Task Should_Have_No_ProductionPlan_When_No_PowerPlant()
    {
        var data = new Production()
        {
            Load = 0,
            Fuel = new Fuel() { GasPricePerMWh = 0, KerosinePricePerMWh = 0, Co2PricePerTon = 0, WindPerCent = 0 },
            PowerPlants = new List<PowerPlant.Application.Domain.PowerPlant>()
            {
            }
        };

        var result = await productionPlanService.CalculateProductionPlan(data, CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().HaveCount(0);
    }

    [TestMethod]
    public async Task Should_Have_One_ProductionPlan_When_One_PowerPlant()
    {
        var data = new Production()
        {
            Load = 100,
            Fuel = new Fuel() { GasPricePerMWh = 0, KerosinePricePerMWh = 0, Co2PricePerTon = 0, WindPerCent = 100 },
            PowerPlants = new List<PowerPlant.Application.Domain.PowerPlant>()
            {
                new PowerPlant.Application.Domain.PowerPlant(){ Name = "A1", Efficiency = 1, ProductionMinimal = 0, ProductionMaximal = 100, Type = PowerType.Windturbine},
            }
        };

        var result = await productionPlanService.CalculateProductionPlan(data, CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().HaveCount(1);
    }

    [TestMethod]
    public async Task Should_All_Product_Zero_When_Load_Is_Zero()
    {
        var data = new Production() 
        { 
            Load = 0, 
            Fuel = new Fuel() { GasPricePerMWh = 50, KerosinePricePerMWh = 50, Co2PricePerTon= 0, WindPerCent = 100},
            PowerPlants = new List<PowerPlant.Application.Domain.PowerPlant>()
            {
                new PowerPlant.Application.Domain.PowerPlant(){ Name = "A1", Efficiency = 1, ProductionMinimal = 0, ProductionMaximal = 100, Type = PowerType.Windturbine},
                new PowerPlant.Application.Domain.PowerPlant(){ Name = "A2", Efficiency = 1, ProductionMinimal = 0, ProductionMaximal = 100, Type = PowerType.GasFired},
                new PowerPlant.Application.Domain.PowerPlant(){ Name = "A3", Efficiency = 1, ProductionMinimal = 0, ProductionMaximal = 100, Type = PowerType.Turbojet}
            }
        };

        var result = await productionPlanService.CalculateProductionPlan(data, CancellationToken.None);

        result.Should().NotBeEmpty();
        result.Should().NotContainNulls();
        result.Should().HaveCount(data.PowerPlants.Count());
        result.Should().AllSatisfy(x => x.Should().HaveProductionIsWithinBound(data));
        result.Sum(x => x.Production).Should().Be(data.Load);
    }

    [TestMethod]
    public async Task Should_Gas_And_Kerosine_Produce_Zero_When_Efficiency_Is_Zero()
    {
        var data = new Production() 
        { 
            Load = 100, 
            Fuel = new Fuel() { GasPricePerMWh = 50, KerosinePricePerMWh = 50, Co2PricePerTon= 0, WindPerCent = 100},
            PowerPlants = new List<PowerPlant.Application.Domain.PowerPlant>()
            {
                new PowerPlant.Application.Domain.PowerPlant(){ Name = "A2", Efficiency = 0, ProductionMinimal = 110, ProductionMaximal = 400, Type = PowerType.GasFired},
                new PowerPlant.Application.Domain.PowerPlant(){ Name = "A3", Efficiency = 0, ProductionMinimal = 110, ProductionMaximal = 400, Type = PowerType.Turbojet}
            }
        };

        var result = await productionPlanService.CalculateProductionPlan(data, CancellationToken.None);

        result.Should().NotBeEmpty();
        result.Should().NotContainNulls();
        result.Should().HaveCount(data.PowerPlants.Count());
        result.Should().AllSatisfy(x => x.Production.Should().Be(0));
        result.Sum(x => x.Production).Should().Be(0);
    }

    [TestMethod]
    public async Task Should_Produce_With_Gas_When_Efficiency_Gas_Is_Better()
    {
        var data = new Production() 
        { 
            Load = 100, 
            Fuel = new Fuel() { GasPricePerMWh = 50, KerosinePricePerMWh = 50, Co2PricePerTon= 0, WindPerCent = 0},
            PowerPlants = new List<PowerPlant.Application.Domain.PowerPlant>()
            {
                new PowerPlant.Application.Domain.PowerPlant(){ Name = "A1", Efficiency = 1m, ProductionMinimal = 0, ProductionMaximal = 400, Type = PowerType.Windturbine},
                new PowerPlant.Application.Domain.PowerPlant(){ Name = "A2", Efficiency = 0.8m, ProductionMinimal = 0, ProductionMaximal = 400, Type = PowerType.GasFired},
                new PowerPlant.Application.Domain.PowerPlant(){ Name = "A3", Efficiency = 0.6m, ProductionMinimal = 0, ProductionMaximal = 400, Type = PowerType.Turbojet}
            }
        };

        var result = await productionPlanService.CalculateProductionPlan(data, CancellationToken.None);

        result.Should().NotBeEmpty();
        result.Should().NotContainNulls();
        result.Should().HaveCount(data.PowerPlants.Count());
        result.Should().AllSatisfy(x => x.Should().HaveProductionIsWithinBound(data));
        result.Should().ContainEquivalentOf(new ProductionPlan() { Name = "A2", Production = 100});
        result.Should().ContainEquivalentOf(new ProductionPlan() { Name = "A3", Production = 0});
        result.Should().ContainEquivalentOf(new ProductionPlan() { Name = "A1", Production = 0});
        result.Sum(x => x.Production).Should().Be(data.Load);
    }

    [TestMethod]
    public async Task Should_Produce_With_Kerosine_When_Efficiency_Kerosine_Is_Better()
    {
        var data = new Production() 
        { 
            Load = 100, 
            Fuel = new Fuel() { GasPricePerMWh = 50, KerosinePricePerMWh = 50, Co2PricePerTon= 0, WindPerCent = 0},
            PowerPlants = new List<PowerPlant.Application.Domain.PowerPlant>()
            {
                new PowerPlant.Application.Domain.PowerPlant(){ Name = "A1", Efficiency = 1m, ProductionMinimal = 0, ProductionMaximal = 400, Type = PowerType.Windturbine},
                new PowerPlant.Application.Domain.PowerPlant(){ Name = "A2", Efficiency = 0.2m, ProductionMinimal = 0, ProductionMaximal = 400, Type = PowerType.GasFired},
                new PowerPlant.Application.Domain.PowerPlant(){ Name = "A3", Efficiency = 0.3m, ProductionMinimal = 0, ProductionMaximal = 400, Type = PowerType.Turbojet}
            }
        };

        var result = await productionPlanService.CalculateProductionPlan(data, CancellationToken.None);

        result.Should().NotBeEmpty();
        result.Should().NotContainNulls();
        result.Should().HaveCount(data.PowerPlants.Count());
        result.Should().AllSatisfy(x => x.Should().HaveProductionIsWithinBound(data));
        result.Should().ContainEquivalentOf(new ProductionPlan() { Name = "A2", Production = 0});
        result.Should().ContainEquivalentOf(new ProductionPlan() { Name = "A3", Production = 100});
        result.Should().ContainEquivalentOf(new ProductionPlan() { Name = "A1", Production = 0 });
        result.Sum(x => x.Production).Should().Be(data.Load);
    }

    [TestMethod]
    public async Task Should_Give_Best_Computed_Production_Plan_When_No_Power_Plan_Can_Give_Enough()
    {
        var data = new Production() 
        { 
            Load = 400, 
            Fuel = new Fuel() { GasPricePerMWh = 50, KerosinePricePerMWh = 50, Co2PricePerTon= 0, WindPerCent = 100},
            PowerPlants = new List<PowerPlant.Application.Domain.PowerPlant>()
            {
                new PowerPlant.Application.Domain.PowerPlant(){ Name = "A1", Efficiency = 1m, ProductionMinimal = 0, ProductionMaximal = 100, Type = PowerType.Windturbine},
                new PowerPlant.Application.Domain.PowerPlant(){ Name = "A2", Efficiency = 1m, ProductionMinimal = 0, ProductionMaximal = 100, Type = PowerType.GasFired},
                new PowerPlant.Application.Domain.PowerPlant(){ Name = "A3", Efficiency = 1m, ProductionMinimal = 0, ProductionMaximal = 100, Type = PowerType.Turbojet}
            }
        };

        var result = await productionPlanService.CalculateProductionPlan(data, CancellationToken.None);

        result.Should().NotBeEmpty();
        result.Should().NotContainNulls();
        result.Should().HaveCount(data.PowerPlants.Count());
        result.Should().AllSatisfy(x => x.Should().HaveProductionIsWithinBound(data));
        result.Should().ContainEquivalentOf(new ProductionPlan() { Name = "A2", Production = 100});
        result.Should().ContainEquivalentOf(new ProductionPlan() { Name = "A3", Production = 100});
        result.Should().ContainEquivalentOf(new ProductionPlan() { Name = "A1", Production = 100 });
    }

    [TestMethod]
    public async Task Should_Windturbine_Product_Zero_When_Wind_Is_Zero()
    {
        var data = new Production() 
        { 
            Load = 100, 
            Fuel = new Fuel() { GasPricePerMWh = 50, KerosinePricePerMWh = 50, Co2PricePerTon= 0, WindPerCent = 0},
            PowerPlants = new List<PowerPlant.Application.Domain.PowerPlant>()
            {
                new PowerPlant.Application.Domain.PowerPlant(){ Name = "A2", Efficiency = 0, ProductionMinimal = 0, ProductionMaximal = 100, Type = PowerType.Windturbine}
            }
        };

        var result = await productionPlanService.CalculateProductionPlan(data, CancellationToken.None);

        result.Should().NotBeEmpty();
        result.Should().NotContainNulls();
        result.Should().HaveCount(data.PowerPlants.Count());
        result.Should().AllSatisfy(x => x.Production.Should().Be(0));
        result.Sum(x => x.Production).Should().Be(0);
    }


    [DataTestMethod]
    [DataRow("example_payloads/payload1.json")]
    [DataRow("example_payloads/payload2.json")]
    [DataRow("example_payloads/payload3.json")]
    [DataRow("example_payloads/payload4.json")]
    public async Task Should_Produce_Response_When_Payload(string file)
    {
        var payload = new JsonDataTest<Production>(file);
        var data = payload.GetData();

        var result = await productionPlanService.CalculateProductionPlan(data, CancellationToken.None);

        result.Should().NotBeEmpty();
        result.Should().NotContainNulls();
        result.Should().HaveCount(data.PowerPlants.Count());
        result.Should().AllSatisfy(x => x.Should().HaveProductionIsWithinBound(data));
        result.Sum(x => x.Production).Should().Be(data.Load);
    }

    [DataTestMethod]
    [DataRow("example_payloads/payload3.json", "example_payloads/response3.json")]
    public async Task Should_Produce_Correct_Response_When_Correct_Payload_Is_Given(string filePayload, string fileResponse)
    {
        var payload = new JsonDataTest<Production>(filePayload);
        var data = payload.GetData();

        var response = new JsonDataTest<ProductionPlan[]>(fileResponse);
        var dataResponse = response.GetData();

        var result = await productionPlanService.CalculateProductionPlan(data, CancellationToken.None);

        result.Should().NotBeEmpty();
        result.Should().NotContainNulls();
        result.Should().HaveCount(data.PowerPlants.Count());
        result.Should().BeEquivalentTo(dataResponse);
        result.Should().AllSatisfy(x => x.Should().HaveProductionIsWithinBound(data));
        result.Sum(x => x.Production).Should().Be(data.Load);
    }
}