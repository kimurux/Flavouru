using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Flavouru.Application.Interfaces;
using Flavouru.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace Flavouru.API.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;

        public ChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        public async Task SendMessage(CreateChatMessageDto messageDto)
        {
            // Отправлятель
            messageDto.SenderId = GetCurrentUserId();

            var sentMessage = await _chatService.SendMessageAsync(messageDto);

            await Clients.Users(
                sentMessage.SenderId.ToString(),
                sentMessage.ReceiverId.ToString()
            ).SendAsync("ReceiveMessage", sentMessage);
        }

        public async Task GetUserMessages(Guid userId)
        {
            var messages = await _chatService.GetUserMessagesAsync(userId);
            await Clients.Caller.SendAsync("LoadMessages", messages);
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = Context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            return userIdClaim != null
                ? Guid.Parse(userIdClaim.Value)
                : throw new UnauthorizedAccessException("User not authenticated");
        }

        public override async Task OnConnectedAsync()
        {
            var userId = GetCurrentUserId();
            await Groups.AddToGroupAsync(Context.ConnectionId, userId.ToString());
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = GetCurrentUserId();
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId.ToString());
            await base.OnDisconnectedAsync(exception);
        }
    }
}