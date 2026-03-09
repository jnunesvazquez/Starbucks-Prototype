using Core.Mappy.Interfaces;
using Core.mediatOR.Contracts;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Starbucks.Application.Abstractions;
using Starbucks.Application.Coffes.DTOs;
using Starbucks.Domain;
using Starbucks.Domain.Abstractions;
using Starbucks.Persistence;

namespace Starbucks.Application.Coffes.Commands;

public class CoffeCreate
{

    public class Command : IRequest<Result<Guid>>
    {
        public required CoffeCreateRequest CoffeCreateRequest {get;set;}
    }

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.CoffeCreateRequest)
            .SetValidator(new RequestValidator());
        }
    }

    public class RequestValidator : AbstractValidator<CoffeCreateRequest>
    {
        public RequestValidator()
        {
           RuleFor(x => x.Name).NotEmpty().WithMessage("El nombre es requerido");
           RuleFor(x => x.Description)
                        .NotEmpty()
                        .WithMessage("La descripcion es requerida");

            RuleFor(x => x.CategoryId)
                        .NotEmpty()
                        .WithMessage("La categoria es requerida");
        }
    }


    public class Handler(
        StarbucksDbContext context, 
        IMapper mapper
    ) 
    : IRequestHandler<Command, Result<Guid>>
    {
        private readonly StarbucksDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        public async Task<Result<Guid>> Handle(
            Command request, 
            CancellationToken cancellationToken
        )
        {
            //var operacionTest = 10 / request.CoffeCreateRequest.Price;

            var nombreCafeIsExists = await _context
                                    .Coffes
                                    .AnyAsync(
                                    x =>
                                    x.Name == request.CoffeCreateRequest.Name
                                    );

            if (nombreCafeIsExists)
            {
                return Result<Guid>.Failure(
                   CoffeErrors.NombreDuplicado
                );
            }

            var coffe = _mapper.Map<Domain.Coffe>(request.CoffeCreateRequest);
            _context.Add(coffe);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(coffe.Id);
        }
    }

}
