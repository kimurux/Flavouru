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
    public class ChatService : IChatService
    {
        private readonly FlavouruDbContext _context;
        private readonly IMapper _mapper;

        public ChatService(FlavouruDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ChatMessageDto>> GetUserMessagesAsync(Guid userId)
        {
            var messages = await _context.ChatMessages
                .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ChatMessageDto>>(messages);
        }

        public async Task<ChatMessageDto> SendMessageAsync(CreateChatMessageDto messageDto)
        {
            var message = _mapper.Map<ChatMessage>(messageDto);
            message.SentAt = DateTime.UtcNow;

            _context.ChatMessages.Add(message);
            await _context.SaveChangesAsync();

            return _mapper.Map<ChatMessageDto>(message);
        }
    }
}

