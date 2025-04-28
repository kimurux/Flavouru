using AutoMapper;
using Flavouru.Domain.Entities;
using Flavouru.Shared.DTOs;

namespace Flavouru.Application.Mappings
{
    public class ChatProfile : Profile
    {
        public ChatProfile()
        {
            CreateMap<ChatMessage, ChatMessageDto>()
                .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src => src.Sender.Username))
                .ForMember(dest => dest.ReceiverName, opt => opt.MapFrom(src => src.Receiver.Username));
            CreateMap<CreateChatMessageDto, ChatMessage>();
        }
    }
}

