using Core.mediatOR.Contracts;
using Core.MediatOR.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Core.mediatOR;

public class Mediator : IMediator
{
    private readonly IServiceProvider _provider;

    public Mediator(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task<TResponse> Send<TResponse>(
        IRequest<TResponse> request, 
        CancellationToken cancellationToken
    )
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var handlerType = typeof(IRequestHandler<,>)
                                .MakeGenericType(
                                    request.GetType(), 
                                    typeof(TResponse)
                                );

        dynamic handler = _provider.GetRequiredService(handlerType);

        if (handler is null)
        {
            throw new InvalidOperationException(
                $"No se encontro el handler para {request.GetType().Name}"
            );
        }


        var behaviorType = typeof(IPipelineBehavior<,>)
                        .MakeGenericType(request.GetType(), typeof(TResponse));

        var behaviors = _provider
                        .GetServices(behaviorType)
                        .Cast<dynamic>()
                        .Reverse().ToList();
        

        RequestHandlerDelegate<TResponse> handlerDelegate = 
                () => handler.Handle((dynamic)request, cancellationToken);

        foreach(var behavior in behaviors) 
        {
            var next = handlerDelegate;
            handlerDelegate = () 
            => behavior.Handle((dynamic)request, cancellationToken, next);
        }

        return await handlerDelegate();
    }
}
