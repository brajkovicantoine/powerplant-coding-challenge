using Microsoft.AspNetCore.Mvc;
using PowerPlan.Application;
using PowerPlant.Application.Domain;

namespace PowerPlant.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductionPlanController : ControllerBase
{
    private readonly ILogger<ProductionPlanController> _logger;
    private readonly IProductionPlanService _productionPlanService;

    public ProductionPlanController(ILogger<ProductionPlanController> logger, IProductionPlanService productionPlanService)
    {
        _logger = logger;
        _productionPlanService = productionPlanService;
    }

    [HttpPost]
    public async Task<IEnumerable<ProductionPlan>> Post([FromBody]Production production)
    {
       return await _productionPlanService.CalculateProductionPlan(production, CancellationToken.None);
    }
}
