using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flavouru.Shared.DTOs;

namespace Flavouru.Application.Interfaces
{
    public interface ITagService
    {
        Task<IEnumerable<TagDto>> GetAllTagsAsync();
        Task<TagDto> CreateTagAsync(CreateTagDto tagDto);
        Task DeleteTagAsync(Guid id);
    }
}

