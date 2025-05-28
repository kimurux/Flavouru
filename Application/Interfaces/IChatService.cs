using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flavouru.Shared.DTOs;

namespace Flavouru.Application.Interfaces
{
    public interface IChatService
    {
        Task<IEnumerable<ChatMessageDto>> GetUserMessagesAsync(Guid userId);
        Task<ChatMessageDto> SendMessageAsync(CreateChatMessageDto messageDto);
    }
}

