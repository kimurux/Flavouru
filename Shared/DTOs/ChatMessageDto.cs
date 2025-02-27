using System;

namespace Flavouru.Shared.DTOs
{
    public class ChatMessageDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public Guid SenderId { get; set; }
        public string SenderName { get; set; }
        public Guid ReceiverId { get; set; }
        public string ReceiverName { get; set; }
    }

    public class CreateChatMessageDto
    {
        public string Content { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
    }
}

