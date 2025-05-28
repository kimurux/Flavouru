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
    public class CategoryService : ICategoryService
    {
        private readonly FlavouruDbContext _context;
        private readonly IMapper _mapper;

        public CategoryService(FlavouruDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _context.Categories.ToListAsync();
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return _mapper.Map<CategoryDto>(category);
        }

        public async Task DeleteCategoryAsync(Guid id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }
    }
}

