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
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<ChatMessageDto>>> GetUserMessages(Guid userId)
        {
            var messages = await _chatService.GetUserMessagesAsync(userId);
            return Ok(messages);
        }

        [HttpPost]
        public async Task<ActionResult<ChatMessageDto>> SendMessage([FromBody] CreateChatMessageDto messageDto)
        {
            var sentMessage = await _chatService.SendMessageAsync(messageDto);
            return CreatedAtAction(nameof(GetUserMessages), new { userId = sentMessage.SenderId }, sentMessage);
        }
    }
}

