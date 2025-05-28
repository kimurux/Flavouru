using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Flavouru.Application.Interfaces;
using Flavouru.Domain.Entities;
using Flavouru.Infrastructure.Data;
using Flavouru.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Flavouru.Infrastructure.Services
{
    public class TagService : ITagService
    {
        private readonly FlavouruDbContext _context;
        private readonly IMapper _mapper;

        public TagService(FlavouruDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TagDto>> GetAllTagsAsync()
        {
            var tags = await _context.Tags.ToListAsync();
            return _mapper.Map<IEnumerable<TagDto>>(tags);
        }

        public async Task<TagDto> CreateTagAsync(CreateTagDto tagDto)
        {
            var tag = _mapper.Map<Tag>(tagDto);
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();
            return _mapper.Map<TagDto>(tag);
        }

        public async Task DeleteTagAsync(Guid id)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag != null)
            {
                _context.Tags.Remove(tag);
                await _context.SaveChangesAsync();
            }
        }
    }
}

