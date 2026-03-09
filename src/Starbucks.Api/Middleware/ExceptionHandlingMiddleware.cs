using Microsoft.AspNetCore.Mvc;
namespace Starbucks.Api.Middleware;
public class ExceptionHandlingMiddleware
{

    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionHandlingMiddleware(
        RequestDelegate next, 
        ILogger<ExceptionHandlingMiddleware> logger, 
        IHostEnvironment env
    )
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch(Exception e)
        {
            _logger.LogError(e, "Este error es una excepcion");
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var problem = new ProblemDetails
            {
                Title = "Ha ocurrido un error inesperado",
                Status = 500,
                Detail = _env.IsDevelopment()
                        ? e.ToString()
                        : "contacte al soporte",
                Instance = context.Request.Path
            };



            await context.Response.WriteAsJsonAsync(problem);
        }
    }

}
