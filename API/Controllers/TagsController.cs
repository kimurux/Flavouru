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
    public class TagsController : ControllerBase
    {
        private readonly ITagService _tagService;

        public TagsController(ITagService tagService)
        {
            _tagService = tagService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TagDto>>> GetAllTags()
        {
            var tags = await _tagService.GetAllTagsAsync();
            return Ok(tags);
        }

        [HttpPost]
        public async Task<ActionResult<TagDto>> CreateTag([FromBody] CreateTagDto createTagDto)
        {
            var createdTag = await _tagService.CreateTagAsync(createTagDto);
            return CreatedAtAction(nameof(GetAllTags), new { id = createdTag.Id }, createdTag);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTag(Guid id)
        {
            await _tagService.DeleteTagAsync(id);
            return NoContent();
        }
    }
}

