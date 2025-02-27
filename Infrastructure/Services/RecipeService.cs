using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flavouru.Domain.Entities;
using Flavouru.Application.Interfaces;
using Flavouru.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Flavouru.Infrastructure.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly FlavouruDbContext _context;

        public RecipeService(FlavouruDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Recipe>> GetAllRecipesAsync()
        {
            return await _context.Recipes
                .Include(r => r.User)
                .Include(r => r.Categories)
                .Include(r => r.Tags)
                .ToListAsync();
        }

        public async Task<Recipe> GetRecipeByIdAsync(Guid id)
        {
            return await _context.Recipes
                .Include(r => r.User)
                .Include(r => r.Categories)
                .Include(r => r.Tags)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Recipe> CreateRecipeAsync(Recipe recipe)
        {
            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();
            return recipe;
        }

        public async Task<Recipe> UpdateRecipeAsync(Recipe recipe)
        {
            _context.Entry(recipe).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return recipe;
        }

        public async Task DeleteRecipeAsync(Guid id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe != null)
            {
                _context.Recipes.Remove(recipe);
                await _context.SaveChangesAsync();
            }
        }
    }
}

