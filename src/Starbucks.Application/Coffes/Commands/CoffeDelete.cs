using Core.mediatOR.Contracts;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Starbucks.Application.Abstractions;
using Starbucks.Domain;
using Starbucks.Persistence;

namespace Starbucks.Application.Coffes.Commands;

public class CoffeDelete
{
    public class Command : IRequest<Result>
    {
        public Guid Id { get; set; }
    }

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty()
            .WithMessage("Debe enviar el id del cafe a eliminar");
        }
    }

    public class Handler(StarbucksDbContext context) : IRequestHandler<Command, Result>
    {
        private readonly StarbucksDbContext _context = context;
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var coffe = await _context
                    .Coffes.FirstOrDefaultAsync(x => x.Id == request.Id);

            if (coffe is null)
            {
                return Result.Failure(CoffeErrors.CoffeNoExiste);
            }

            _context.Coffes.Remove(coffe);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
