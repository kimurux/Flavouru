using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Flavouru.Application.Interfaces;
using Flavouru.Domain.Entities;
using Flavouru.Infrastructure.Data;
using Flavouru.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Flavouru.Infrastructure.Services
{
    public class CommentService : ICommentService
    {
        private readonly FlavouruDbContext _context;
        private readonly IMapper _mapper;

        public CommentService(FlavouruDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CommentDto>> GetCommentsByRecipeAsync(Guid recipeId)
        {
            var comments = await _context.Comments
                .Where(c => c.RecipeId == recipeId)
                .Include(c => c.User)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<CommentDto>>(comments);
        }

        public async Task<CommentDto> CreateCommentAsync(CreateCommentDto commentDto)
        {
            var comment = _mapper.Map<Comment>(commentDto);
            comment.CreatedAt = DateTime.UtcNow;

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return _mapper.Map<CommentDto>(comment);
        }

        public async Task<CommentDto> UpdateCommentAsync(Guid id, UpdateCommentDto commentDto)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return null;
            }

            _mapper.Map(commentDto, comment);
            await _context.SaveChangesAsync();

            return _mapper.Map<CommentDto>(comment);
        }

        public async Task DeleteCommentAsync(Guid id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
            }
        }
    }
}

