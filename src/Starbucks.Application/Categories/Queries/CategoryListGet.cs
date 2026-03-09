using Core.Mappy.Interfaces;
using Core.mediatOR.Contracts;
using Microsoft.EntityFrameworkCore;
using Starbucks.Application.Abstractions;
using Starbucks.Application.Categories.DTOs;
using Starbucks.Persistence;

namespace Starbucks.Application.Categories.Queries;

public class CategoryListGet
{

    public class Query : IRequest<Result<List<CategoryResponse>>>
    {}

    public class Handler(
        StarbucksDbContext context,
        IMapper mapper
    ) 
    : IRequestHandler<Query, Result<List<CategoryResponse>>>
    {
        private readonly StarbucksDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        public async Task<Result<List<CategoryResponse>>> Handle(
            Query request,
            CancellationToken cancellationToken
        )
        {
            var categories = await _context.Categories.ToListAsync();
            var categoryResponse = _mapper
                    .Map<List<CategoryResponse>>(categories);

            return Result<List<CategoryResponse>>.Success(categoryResponse);
        }
    }
}
