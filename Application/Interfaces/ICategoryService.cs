using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flavouru.Shared.DTOs;

namespace Flavouru.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto categoryDto);
        Task DeleteCategoryAsync(Guid id);
    }
}

