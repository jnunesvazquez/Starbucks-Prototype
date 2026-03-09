using System;

namespace Starbucks.Application.Categories.DTOs;

public class CategoryResponse
{
    public int CategoryId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
}
