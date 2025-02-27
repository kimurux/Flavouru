using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flavouru.Domain.Entities;

namespace Flavouru.Application.Interfaces
{
    public interface IRecipeService
    {
        Task<Recipe> GetRecipeByIdAsync(Guid id);
        Task<IEnumerable<Recipe>> GetAllRecipesAsync();
        Task<Recipe> CreateRecipeAsync(Recipe recipe);
        Task<Recipe> UpdateRecipeAsync(Recipe recipe);
        Task DeleteRecipeAsync(Guid id);
    }
}

