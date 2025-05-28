using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flavouru.Domain.Entities;
using Flavouru.Shared.DTOs;

namespace Flavouru.Application.Interfaces
{
    public interface ICommentService
    {
        Task<IEnumerable<CommentDto>> GetCommentsByRecipeAsync(Guid recipeId);
        Task<CommentDto> CreateCommentAsync(CreateCommentDto commentDto);
        Task<CommentDto> UpdateCommentAsync(Guid id, UpdateCommentDto commentDto);
        Task DeleteCommentAsync(Guid id);
    }
}

