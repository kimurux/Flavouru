using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flavouru.Application.Interfaces;
using Flavouru.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Flavouru.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet("recipe/{recipeId}")]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetCommentsByRecipe(Guid recipeId)
        {
            var comments = await _commentService.GetCommentsByRecipeAsync(recipeId);
            return Ok(comments);
        }

        [HttpPost]
        public async Task<ActionResult<CommentDto>> CreateComment([FromBody] CreateCommentDto createCommentDto)
        {
            var createdComment = await _commentService.CreateCommentAsync(createCommentDto);
            return CreatedAtAction(nameof(GetCommentsByRecipe), new { recipeId = createdComment.RecipeId }, createdComment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(Guid id, [FromBody] UpdateCommentDto updateCommentDto)
        {
            var updatedComment = await _commentService.UpdateCommentAsync(id, updateCommentDto);
            if (updatedComment == null)
            {
                return NotFound();
            }
            return Ok(updatedComment);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            await _commentService.DeleteCommentAsync(id);
            return NoContent();
        }
    }
}

