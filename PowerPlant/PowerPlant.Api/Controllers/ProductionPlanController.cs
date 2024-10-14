using MediatR;
using Microsoft.AspNetCore.Mvc;
using PowerPlan.Application;

namespace PowerPlant.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductionPlanController : ControllerBase
{
    private readonly ILogger<ProductionPlanController> _logger;
    private readonly IMediator _mediator;

    public ProductionPlanController(ILogger<ProductionPlanController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IEnumerable<ProductionPlan>> Post([FromBody]Production production)
    {
        return await _mediator.Send(new ProductionPlanRequest(production));
    }
}
