using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flavouru.Domain.Entities;
using Flavouru.Application.Interfaces;
using Flavouru.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Flavouru.Infrastructure.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly FlavouruDbContext _context;
        private readonly ILogger<RecipeService> _logger;

        public RecipeService(
            FlavouruDbContext context,
            ILogger<RecipeService> logger)
        {
            _context = context;
            _logger = logger;
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
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = await _context.Users.FindAsync(recipe.UserId);
                if (user == null)
                {
                    _logger.LogWarning($"Attempted to create recipe for non-existent user ID: {recipe.UserId}");
                    throw new InvalidOperationException($"User with ID {recipe.UserId} does not exist");
                }

                recipe.CreatedAt = DateTime.UtcNow;
                _context.Recipes.Add(recipe);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return recipe;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error creating recipe");
                throw;
            }
        }

        public async Task<Recipe> UpdateRecipeAsync(Recipe recipe)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                recipe.UpdatedAt = DateTime.UtcNow;
                _context.Entry(recipe).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return recipe;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error updating recipe");
                throw;
            }
        }

        public async Task DeleteRecipeAsync(Guid id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var recipe = await _context.Recipes.FindAsync(id);
                if (recipe != null)
                {
                    _context.Recipes.Remove(recipe);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                }
                else
                {
                    _logger.LogWarning($"Рецепт: {id}");
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Error deleting recipe {id}");
                throw;
            }
        }
    }
}