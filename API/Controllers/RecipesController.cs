using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Flavouru.Application.Interfaces;
using Flavouru.Domain.Entities;
using Flavouru.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Flavouru.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace Flavouru.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecipesController : ControllerBase
    {
        private readonly IRecipeService _recipeService;
        private readonly IMapper _mapper;
        private readonly FlavouruDbContext _context;
        private readonly ILogger<RecipesController> _logger;

        public RecipesController(
            IRecipeService recipeService,
            IMapper mapper,
            FlavouruDbContext context,
            ILogger<RecipesController> logger)
        {
            _recipeService = recipeService;
            _mapper = mapper;
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecipeDto>>> GetAllRecipes()
        {
            var recipes = await _recipeService.GetAllRecipesAsync();
            return Ok(_mapper.Map<IEnumerable<RecipeDto>>(recipes));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RecipeDto>> GetRecipe(Guid id)
        {
            var recipe = await _recipeService.GetRecipeByIdAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<RecipeDto>(recipe));
        }

        [HttpPost]
        public async Task<ActionResult<RecipeDto>> CreateRecipe([FromBody] CreateRecipeDto createRecipeDto)
        {
            try
            {
                var userExists = await _context.Users.AnyAsync(u => u.Id == createRecipeDto.UserId);
                if (!userExists)
                {
                    _logger.LogWarning($"Attempt to create recipe for non-existent user: {createRecipeDto.UserId}");
                    return BadRequest($"User with ID {createRecipeDto.UserId} does not exist.");
                }

                var recipe = _mapper.Map<Recipe>(createRecipeDto);

                if (createRecipeDto.CategoryIds != null && createRecipeDto.CategoryIds.Any())
                {
                    recipe.Categories = await _context.Categories
                        .Where(c => createRecipeDto.CategoryIds.Contains(c.Id))
                        .ToListAsync();
                }

                if (createRecipeDto.TagIds != null && createRecipeDto.TagIds.Any())
                {
                    recipe.Tags = await _context.Tags
                        .Where(t => createRecipeDto.TagIds.Contains(t.Id))
                        .ToListAsync();
                }

                recipe.CreatedAt = DateTime.UtcNow;

                var createdRecipe = await _recipeService.CreateRecipeAsync(recipe);
                return CreatedAtAction(nameof(GetRecipe), new { id = createdRecipe.Id }, _mapper.Map<RecipeDto>(createdRecipe));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating recipe");
                return StatusCode(500, "An error occurred while creating the recipe.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRecipe(Guid id, [FromBody] UpdateRecipeDto updateRecipeDto)
        {
            try
            {
                var existingRecipe = await _recipeService.GetRecipeByIdAsync(id);
                if (existingRecipe == null)
                {
                    return NotFound();
                }

                _mapper.Map(updateRecipeDto, existingRecipe);

                // Update categories if provided
                if (updateRecipeDto.CategoryIds != null)
                {
                    existingRecipe.Categories = await _context.Categories
                        .Where(c => updateRecipeDto.CategoryIds.Contains(c.Id))
                        .ToListAsync();
                }

                // Update tags if provided
                if (updateRecipeDto.TagIds != null)
                {
                    existingRecipe.Tags = await _context.Tags
                        .Where(t => updateRecipeDto.TagIds.Contains(t.Id))
                        .ToListAsync();
                }

                existingRecipe.UpdatedAt = DateTime.UtcNow;

                var updatedRecipe = await _recipeService.UpdateRecipeAsync(existingRecipe);
                return Ok(_mapper.Map<RecipeDto>(updatedRecipe));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating recipe");
                return StatusCode(500, "An error occurred while updating the recipe.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecipe(Guid id)
        {
            try
            {
                await _recipeService.DeleteRecipeAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting recipe");
                return StatusCode(500, "An error occurred while deleting the recipe.");
            }
        }
    }
}