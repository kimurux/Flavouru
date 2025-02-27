using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Flavouru.Application.Interfaces;
using Flavouru.Domain.Entities;
using Flavouru.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Flavouru.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecipesController : ControllerBase
    {
        private readonly IRecipeService _recipeService;
        private readonly IMapper _mapper;

        public RecipesController(IRecipeService recipeService, IMapper mapper)
        {
            _recipeService = recipeService;
            _mapper = mapper;
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var recipe = _mapper.Map<Recipe>(createRecipeDto);
            var createdRecipe = await _recipeService.CreateRecipeAsync(recipe);
            return CreatedAtAction(nameof(GetRecipe), new { id = createdRecipe.Id }, _mapper.Map<RecipeDto>(createdRecipe));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRecipe(Guid id, [FromBody] UpdateRecipeDto updateRecipeDto)
        {
            var existingRecipe = await _recipeService.GetRecipeByIdAsync(id);
            if (existingRecipe == null)
            {
                return NotFound();
            }

            _mapper.Map(updateRecipeDto, existingRecipe);
            var updatedRecipe = await _recipeService.UpdateRecipeAsync(existingRecipe);
            return Ok(_mapper.Map<RecipeDto>(updatedRecipe));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecipe(Guid id)
        {
            await _recipeService.DeleteRecipeAsync(id);
            return NoContent();
        }
    }
}

