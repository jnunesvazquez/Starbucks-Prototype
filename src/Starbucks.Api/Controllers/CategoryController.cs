using Core.mediatOR.Contracts;
using Microsoft.AspNetCore.Mvc;
using static Starbucks.Application.Categories.Queries.CategoryListGet;

namespace Starbucks.Api.Controllers;

[Route("api/categories")]
[ApiController]
public class CategoryController(IMediator mediator) : ControllerBase
{
    
    private readonly IMediator _mediator = mediator;
   

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
       var query = new Query();
       var result = await _mediator.Send(query, cancellationToken);
       return Ok(result.Value);
    }
}
